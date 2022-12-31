using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnboundLib;
using ModsPlus;
using ModdingUtils.Utils;

public class TealAura : MonoBehaviour
{
    [SerializeField]
    private float maxRadius, force, minPercentage, fillTime, effectInterval, healthMultiplier, damageMultiplier;
    private float fillTimeCounter;
    private Vector3 baseScale;
    private Player owner;

    private float smoothingVelocity = 0;
    private float targetPercentage = 0;
    private float currentPercentage = 0;
    private float smoothTime = 0.25f;

    [SerializeField]
    private UnityEvent onEffectUpgrade, onEffectDowngrade;

    private List<StatChangeTracker> statChanges = new List<StatChangeTracker>();

    private Vector2 playerVelocity => (Vector2)owner.data.playerVel.GetFieldValue("velocity");

    private StatChanges hermitBuff;

    private void Awake()
    {
        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;

        owner = GetComponentInParent<Player>();
    }

    private void Start()
    {
        hermitBuff = new StatChanges
        {
            MaxHealth = healthMultiplier,
            Damage = damageMultiplier,
        };
    }

    private void OnEnable()
    {
        targetPercentage = currentPercentage = smoothingVelocity = 0;
        StartCoroutine(AuraEffect());
    }

    private IEnumerator AuraEffect()
    {
        while (true)
        {
            yield return new WaitUntil(() => PlayerStatus.PlayerAliveAndSimulated(owner));

            if (targetPercentage >= 1)
            {
                var statAdded = StatManager.Apply(owner, hermitBuff);
                statChanges.Add(statAdded);

                onEffectUpgrade?.Invoke();
                yield return new WaitForSeconds(effectInterval);
            }
            else if (statChanges.Count > 0)
            {
                // clear orphaned stat changes
                statChanges.RemoveAll(s => s == null);
                if (statChanges.Count == 0) continue;

                var statToRemove = statChanges[statChanges.Count - 1];
                statChanges.Remove(statToRemove);
                StatManager.Remove(statToRemove);

                onEffectDowngrade?.Invoke();

                // wait until we can apply or remove the next effect
                float waitTime = Time.time;
                yield return new WaitUntil(() =>
                {
                    return Time.time - waitTime >= effectInterval * 2 || (targetPercentage >= 1 && (Time.time - waitTime >= effectInterval));
                });
            }
            else
            {
                yield return null;
            }
        }
        ClearBuffs();
    }
    
    private void Update()
    {
        currentPercentage = Mathf.SmoothDamp(currentPercentage, targetPercentage, ref smoothingVelocity, smoothTime);
        transform.localScale = baseScale * currentPercentage;
    }

    private void FixedUpdate()
    {
        if (playerVelocity.magnitude < 2)
        {
            fillTimeCounter = Mathf.Min(fillTimeCounter + Time.fixedDeltaTime, fillTime);
        }
        else
        {
            fillTimeCounter = Mathf.Max(fillTimeCounter - Time.fixedDeltaTime * 3, 0);
        }

        targetPercentage = fillTimeCounter / fillTime;

        PushProjectiles();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        ClearBuffs();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        ClearBuffs();
    }

    private float GetScaledRadius()
    {
        return maxRadius * baseScale.x;
    }

    private float GetScaledForce()
    {
        return force * currentPercentage;
    }

    private void PushProjectiles()
    {
        foreach (var hit in Physics2D.OverlapCircleAll(transform.position, GetScaledRadius() * currentPercentage, ArcanaCardsPlugin.projectileMask))
        {
            var force = Vector3.Normalize(hit.transform.position - transform.position) * GetScaledForce() * 0.002f;
            var bulletMover = hit.transform.root.GetComponentInChildren<MoveTransform>();
            bulletMover.velocity += force;
        }
    }

    public void ClearBuffs()
    {
        statChanges.RemoveAll(s => s == null);
        foreach (var buff in statChanges)
        {
            StatManager.Remove(buff);
        }
        statChanges.Clear();
    }
}
