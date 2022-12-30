using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

[RequireComponent(typeof(ParticleSystem))]
public class MagicMissiles : MonoBehaviour
{
    [SerializeField]
    private ParticleSystemForceField attractorPrefab;
    
    [SerializeField]
    private float damage;

    [SerializeField]
    private UnityEvent onDamage;

    [SerializeField]
    private GameObject hitEffect;

    private ParticleSystemForceField attractor;
    private ParticleSystem particles;
    private SpawnedAttack spawned;

    private Player target;

    void Awake()
    {
        if (!((DefaultPool)PhotonNetwork.PrefabPool).ResourceCache.ContainsKey(hitEffect.name))
        {
            PhotonNetwork.PrefabPool.RegisterPrefab(hitEffect.name, hitEffect);
        }
    }

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        spawned = GetComponent<SpawnedAttack>();

        target = PlayerManager.instance.players
            .Where(p => p.teamID != spawned.spawner.teamID && !p.data.dead)
            .OrderBy(p => Vector3.Distance(transform.position, p.transform.position))
            .FirstOrDefault();

        if (target == null) return;

        // add particle attractor
        attractor = Instantiate(attractorPrefab, target.transform);
        particles.externalForces.AddInfluence(attractor);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (target == null || (!PhotonNetwork.IsMasterClient && !PhotonNetwork.OfflineMode)) return;

        var damage = Vector3.Normalize(target.transform.position - spawned.spawner.transform.position) * GetScaledDamage();
        target.data.healthHandler.CallTakeDamage(damage, target.transform.position, damagingPlayer: spawned.spawner);

        onDamage?.Invoke();
        
        // extract collision events for particle velocity
        var events = new List<ParticleCollisionEvent>(particles.GetSafeCollisionEventSize());
        int count = particles.GetCollisionEvents(other, events);
        PhotonNetwork.Instantiate(hitEffect.name, events[0].intersection, hitEffect.transform.rotation);
    }

    private void OnParticleSystemStopped()
    {
        if (attractor != null) Destroy(attractor.gameObject);
        Destroy(gameObject);
    }

    private float GetScaledDamage()
    {
        var cooldown = Mathf.Max(1f, spawned.spawner.data.block.Cooldown());
        var blockCount = Mathf.Max(1f, spawned.spawner.data.block.additionalBlocks);
        return damage * (cooldown / 4f) / blockCount;
    }
}
