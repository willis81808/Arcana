using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ParticleSystem))]
public class DamagingParticles : MonoBehaviour
{
    public float damage;

    public UnityEvent onCollide;

    private ParticleSystem particles;
    private SpawnedAttack spawnedAttack;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        spawnedAttack = GetComponent<SpawnedAttack>();
    }

    private void OnParticleCollision(GameObject other)
    {
        onCollide?.Invoke();

        if (!PhotonNetwork.IsMasterClient && !PhotonNetwork.OfflineMode) return;

        // try to find a damagable component on the hit object
        Damagable target = other.GetComponentInParent<Damagable>();
        if (target == null)
        {
            target = other.GetComponentInChildren<Damagable>();
        }

        // exit if there is no damagable target, or we are trying to damage ourselves
        if (target == null || target.transform.root == spawnedAttack.spawner.transform.root) return;
        
        // extract collision events for particle velocity
        var events = new List<ParticleCollisionEvent>(particles.GetSafeCollisionEventSize());
        int count = particles.GetCollisionEvents(other, events);
        if (count == 0) return;

        // deal damage to target
        target.CallTakeDamage(events[0].velocity.normalized * GetScaledDamage(), events[0].intersection);
    }

    private float GetScaledDamage()
    {
        // scale based on gun damage
        return damage * spawnedAttack.spawner.data.weaponHandler.gun.damage;
    }
}
