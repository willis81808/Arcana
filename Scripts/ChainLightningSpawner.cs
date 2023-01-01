using Photon.Pun;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModsPlus;

public class ChainLightningSpawner : MonoBehaviour
{
    [SerializeField, AssetsOnly]
    internal GameObject chainLightningPrefab;

    private void Awake()
    {
        if (!((DefaultPool)PhotonNetwork.PrefabPool).ResourceCache.ContainsKey(chainLightningPrefab.name))
        {
            PhotonNetwork.PrefabPool.RegisterPrefab(chainLightningPrefab.name, chainLightningPrefab);
        }
    }

    private void Start()
    {
        var owner = GetComponent<SpawnedAttack>().spawner;

        if (!owner.data.view.IsMine) return;

        var targets = PlayerManager.instance.players
            .Where(p => p != owner && p.IsFullyAlive() && PlayerManager.instance.CanSeePlayer(transform.position, p).canSee)
            .OrderBy(p => Vector3.Distance(transform.position, p.transform.position));

        if (targets == null || targets.Count() == 0) return;

        foreach (var target in targets)
        {
            PhotonNetwork.Instantiate(
                chainLightningPrefab.name,
                transform.position,
                Quaternion.identity,
                data: new object[]
                {
                    owner.playerID,
                    new [] { owner.playerID },
                    target.playerID
                });
        }
    }
}
