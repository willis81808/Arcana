using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using UnboundLib.Extensions;
using ModsPlus;
using Photon.Pun;
using System;
using UnityEngine.Events;

public class CrimsonAura : MonoBehaviour
{
    [SerializeField]
    private float maxRadius, minPercentage, fillTime, damage, force;
    private Vector3 baseScale;
    private Player owner;

    private float smoothingVelocity = 0;
    private float targetPercentage = 0;
    private float currentPercentage = 0;
    private float smoothTime = 0.3f;

    [SerializeField]
    private UnityEvent onDealtDamage, onSpawn, onStartDespawn;
    
    private IEnumerator Start()
    {
        // clamp scale to 4x base size
        transform.localScale = Vector3.ClampMagnitude(transform.localScale, 4f);

        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;
        owner = GetComponent<SpawnedAttack>().spawner;

        onSpawn?.Invoke();
        targetPercentage = 1f;
        yield return new WaitForSeconds(fillTime + 1f);

        onStartDespawn?.Invoke();
        targetPercentage = 0f;
        yield return new WaitForSeconds(fillTime / 2);
        Destroy(gameObject);
    }

    private void Update()
    {
        currentPercentage = Mathf.SmoothDamp(currentPercentage, targetPercentage, ref smoothingVelocity, smoothTime);
        transform.localScale = baseScale * currentPercentage;
    }

    private void FixedUpdate()
    {
        ApplyEffectInArea();
    }
    
    private void ApplyEffectInArea()
    {
        if (!PhotonNetwork.IsMasterClient && !PhotonNetwork.OfflineMode) return;

        var damaged = new HashSet<Player>();
        foreach (var hit in Physics2D.OverlapCircleAll(transform.position, GetScaledRadius() * currentPercentage, ArcanaCardsPlugin.playerMask))
        {
            var target = hit.transform.GetComponentInParent<Player>();
            if (damaged.Contains(target)) continue;

            if (!target.gameObject.TryGetComponent(out DOTDealer dotDealer))
            {
                target.gameObject.AddComponent<DOTDealer>().Initialize(
                    owner,
                    target,
                    0.3f,
                    0.3f,
                    transform.position,
                    GetScaledForce,
                    GetScaledDamage,
                    OnDealtDamage
                );
            }

            damaged.Add(target);
        }
    }
    
    private float GetScaledRadius()
    {
        return maxRadius * baseScale.x;
    }

    private float GetScaledForce()
    {
        return force * currentPercentage;
    }

    private float GetScaledDamage()
    {
        var radiusScalar = Mathf.Max(1f, baseScale.x);
        return damage * currentPercentage * radiusScalar;
    }

    private void OnDealtDamage()
    {
        onDealtDamage?.Invoke();
    }
}

public class DOTDealer : MonoBehaviour
{
    private Player owner;
    private Player target;
    private float duration, interval;
    private Vector3 effectCenter;
    private Func<float> damageProvider;
    private Func<float> forceProvider;
    private Action onDealtDamageAction;

    public void Initialize(
        Player owner,
        Player target,
        float duration,
        float interval,
        Vector3 effectCenter,
        Func<float> forceProvider,
        Func<float> damageProvider,
        Action onDealtDamageAction)
    {
        this.owner = owner;
        this.target = target;
        this.duration = duration;
        this.interval = interval;
        this.effectCenter = effectCenter;
        this.forceProvider = forceProvider;
        this.damageProvider = damageProvider;
        this.onDealtDamageAction = onDealtDamageAction;
    }
    
    private IEnumerator Start()
    {
        float startTime = Time.time;
        while (Time.time - startTime <= duration)
        {
            if (target != owner)
            {
                target.data.healthHandler.CallTakeDamage(
                    Vector3.Normalize(effectCenter - target.transform.position) * damageProvider.Invoke(),
                    target.transform.position,
                    damagingPlayer: owner
                );
                onDealtDamageAction?.Invoke();
            }
            yield return new WaitForSeconds(interval);
        }
        Destroy(this);
    }

    private void FixedUpdate()
    {
        var direction = Vector3.Normalize(target.transform.position - effectCenter);
        target.data.healthHandler.CallTakeForce(direction * forceProvider.Invoke(), ForceMode2D.Force);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        Destroy(this);
    }
}