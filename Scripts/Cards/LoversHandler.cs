using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using ModsPlus;
using Photon.Pun;

public class LoversHandler : PlayerHook
{
    [SerializeField]
    private LineEffect circleEffect;

    [SerializeField]
    private GameObject negatedEffect;

    [SerializeField, Range(0f, 1f)]
    private float negationChance;

    protected override void Awake()
    {
        base.Awake();
        if (circleEffect == null)
        {
            circleEffect = GetComponent<LineEffect>();
        }
    }

    private void Update()
    {
        foreach (var hit in Physics2D.OverlapCircleAll(transform.position, circleEffect.radius, ArcanaCardsPlugin.projectileMask))
        {
            // skip if this isn't a bullet
            if (!hit.transform.root.CompareTag("Bullet")) continue;

            // don't negate our own bullets
            if (hit.GetComponentInParent<SpawnedAttack>() is SpawnedAttack attack && attack.spawner.teamID == player.teamID) continue;

            // skip if we've already evaluated this bullet
            var ignore = hit.gameObject.GetOrAddComponent<LoversIgnore>();
            if (ignore.ignorePlayers.Contains(player))
            {
                continue;
            }
            else
            {
                ignore.ignorePlayers.Add(player);
            }

            // skip if negation roll fails
            var view = hit.GetComponentInParent<PhotonView>();
            var rng = new System.Random(view.ViewID);
            if (rng.NextDouble() >= negationChance) continue;

            // attempt to destroy the bullet
            hit.GetComponentInParent<SpawnedAttack>().gameObject.SetActive(false);
            if (view.IsMine)
            {
                PhotonNetwork.Destroy(view);
            }
            SpawnNegatedEffect(hit.transform.position);
        }
    }

    private void SpawnNegatedEffect(Vector3 negatedPosition)
    {
        Instantiate(negatedEffect, negatedPosition, negatedEffect.transform.rotation);
    }

    private class LoversIgnore : MonoBehaviour
    {
        public List<Player> ignorePlayers = new List<Player>();
    }
}
