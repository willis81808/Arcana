using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class MoonBeam : MonoBehaviour
{
    [SerializeField]
    private float delay, duration, force, damage, radius;

    [SerializeField]
    private UnityEvent onEffectActivate, onEffectDeactivate;

    private SpawnedAttack spawned;

    IEnumerator Start()
    {
        spawned = GetComponent<SpawnedAttack>();

        yield return new WaitForSeconds(delay);
        
        onEffectActivate?.Invoke();

        // stun and damage players in radius
        var playersHit = PlayerManager.instance.players
            .Where(p => p.playerID != spawned.spawner.playerID && !p.data.dead)
            .Where(p => Vector3.Distance(transform.position, p.transform.position) <= GetScaledRadius());
        foreach (var target in playersHit)
        {
            target.data.stunHandler.AddStun(1f);

            if (PhotonNetwork.IsMasterClient || PhotonNetwork.OfflineMode)
            {
                var direction = Vector3.Normalize(target.transform.position - transform.position);

                // push target
                target.data.healthHandler.CallTakeForce(direction * GetScaledForce(), ForceMode2D.Impulse, ignoreBlock: true);

                // deal damage
                target.data.healthHandler.CallTakeDamage(direction * GetScaledDamage(), target.transform.position, damagingPlayer: spawned.spawner);
            }
        }

        // push projectiles away in radius
        var startTime = Time.time;
        while (Time.time - startTime <= duration)
        {
            yield return new WaitForFixedUpdate();
            PushProjectiles();
        }

        onEffectDeactivate?.Invoke();
    }
    
    private void PushProjectiles()
    {
        foreach (var hit in Physics2D.OverlapCircleAll(transform.position, GetScaledRadius(), ArcanaCardsPlugin.projectileMask))
        {
            var force = Vector3.Normalize(hit.transform.position - transform.position) * GetScaledForce() * 0.02f;
            var bulletMover = hit.transform.root.GetComponentInChildren<MoveTransform>();
            bulletMover.velocity += force;
        }
    }

    private float GetHealthScalar()
    {
        return spawned.spawner.data.maxHealth / 100f;
    }

    private float GetScaledForce()
    {
        return force * GetHealthScalar();
    }

    private float GetScaledDamage()
    {
        return damage * GetHealthScalar();
    }

    private float GetScaledRadius()
    {
        return radius * transform.localScale.x;
    }
}
