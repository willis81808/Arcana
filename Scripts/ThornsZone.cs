using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnboundLib.Extensions;
using UnboundLib;
using ModsPlus;
using UnityEngine.Events;
using Photon.Pun;

public class ThornsZone : MonoBehaviour
{
    private float damageScalar;

    [SerializeField]
    private UnityEvent onDamage;

    private Player owner;

    public void Initialize(Player owner, float damageScalar)
    {
        this.owner = owner;
        this.damageScalar = damageScalar;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<Player>() is Player target && target != owner)
        {
            target.gameObject.GetOrAddComponent<ThornsDebuff>().Initialize(owner, damageScalar, onDamage.Invoke);
        }
    }
}

public class ThornsDebuff : PlayerHook
{
    private StatChanges debuff = new StatChanges
    {
        MovementSpeed = 0.75f,
    };

    private StatChangeTracker appliedDebuff;

    private Player owner;
    private Action onDamage;

    private const float
        baseDamage = 5f,
        duration = 1f;
    
    private float damageScalar = 1f;

    public void Initialize(Player owner, float damageScalar, Action onDamage)
    {
        this.owner = owner;
        this.damageScalar = damageScalar;
        this.onDamage = onDamage;
    }

    protected override void Start()
    {
        base.Start();
        appliedDebuff = StatManager.Apply(player, debuff);
        StartCoroutine(ThornsDamage(duration, 0.2f));
    }

    private IEnumerator ThornsDamage(float duration, float interval)
    {
        float cooldown = 0;
        float counter = 0;
        while (counter <= duration)
        {
            float delta = Time.deltaTime * TimeHandler.timeScale;
            counter += delta;
            cooldown += delta;

            if (cooldown >= interval)
            {
                if (owner.data.view.IsMine)
                {
                    player.data.healthHandler.CallTakeDamage(Vector2.up * GetScaledDamage(), player.transform.position, damagingPlayer: owner);
                }
                onDamage?.Invoke();
                cooldown = 0;
            }
            yield return null;
        }

        Destroy(this);
    }

    private float GetHealthScalar()
    {
        return owner.data.maxHealth / 100f;
    }

    private float GetScaledDamage()
    {
        var scalar = owner.data.weaponHandler.gun.damage * GetHealthScalar() * damageScalar;
        return baseDamage * scalar;
    }

    private void OnDisable()
    {
        Destroy(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        StopAllCoroutines();
        StatManager.Remove(appliedDebuff);
    }
}
