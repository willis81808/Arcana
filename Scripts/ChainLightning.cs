using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnboundLib;
using ModsPlus;

[RequireComponent(typeof(PhotonView))]
public class ChainLightning : MonoBehaviour, IPunInstantiateMagicCallback
{
    [SerializeField, AssetsOnly]
    private ChainLightningSpawner spawner;

    [SerializeField]
    private UnityEvent onHit;

    [SerializeField]
    private float hitRange, damage;

    private PhotonView view;

    private HashSet<Player> hitPlayers = new HashSet<Player>();
    private Player target, owner;

    private Vector2 velocity;
    private bool active = true;
    
    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        if (!view.IsMine || !active) return;
        
        transform.position = Vector2.SmoothDamp(transform.position, target.transform.position, ref velocity, 0.1f);
        view.RPC(nameof(RPC_SetPosition), RpcTarget.Others, transform.position);

        if (InRange())
        {
            hitPlayers.Add(target);

            TrySplit();

            view.RPC(nameof(RPCA_DealDamage), RpcTarget.All, velocity, GetScaledDamage(), transform.position, owner.playerID);
            Unbound.Instance.ExecuteAfterFrames(1, () => PhotonNetwork.Destroy(gameObject));
            active = false;
        }
    }

    private bool InRange()
    {
        return Vector3.Distance(transform.position, target.transform.position) <= hitRange;
    }

    private float GetScaledDamage()
    {
        return damage * owner.data.weaponHandler.gun.damage;
    }

    [PunRPC]
    private void RPCA_DealDamage(Vector2 velocity, float damage, Vector3 position, int ownerId)
    {
        var owner = PlayerManager.instance.GetPlayerWithID(ownerId);
        target.data.healthHandler.TakeDamage(velocity.normalized * damage, position, damagingPlayer: owner);
        onHit?.Invoke();
    }
    
    private void TrySplit()
    {
        var newTargets = PlayerManager.instance.players
            .Except(hitPlayers)
            .Where(p => p.IsFullyAlive() && PlayerManager.instance.CanSeePlayer(transform.position, p).canSee);

        foreach (var newTarget in newTargets)
        {
            PhotonNetwork.Instantiate(
                spawner.chainLightningPrefab.name,
                transform.position,
                Quaternion.identity,
                data: new object[]
                {
                    owner.playerID,
                    hitPlayers.Select(p => p.playerID).ToArray(),
                    newTarget.playerID
                });
        }
    }

    [PunRPC]
    private void RPC_SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var args = info.photonView.InstantiationData;
        if (args.Length != 3) return;

        var ownerPlayerId = (int)args[0];
        var ignorePlayerIds = (int[])args[1];
        var targetPlayerId = (int)args[2];
        
        // extract owner
        owner = PlayerManager.instance.GetPlayerWithID(ownerPlayerId);

        // extract player hit history (avoids infinite chaining)
        var ignorePlayers = PlayerManager.instance.players.Where(p => ignorePlayerIds.Contains(p.playerID));
        hitPlayers.UnionWith(ignorePlayers);

        // extract new target
        target = PlayerManager.instance.GetPlayerWithID(targetPlayerId);
    }
}
