using System.Collections;
using System.Collections.Generic;
using UnboundLib;
using UnboundLib.Extensions;
using UnityEngine;

public class ChariotCharge : MonoBehaviour
{
    [SerializeField]
    private float damage, castRadius, castRange;

    private SpawnedAttack spawned;

    void Awake()
    {
        spawned = GetComponent<SpawnedAttack>();
    }

    IEnumerator Start()
    {
        yield return null;

        transform.up = spawned.spawner.data.aimDirection;

        if (!spawned.spawner.data.view.IsMine) yield break;

        yield return new WaitForSeconds(0.15f);

        foreach (var hit in Physics2D.CircleCastAll(transform.position, castRadius, transform.up, castRange, ArcanaCardsPlugin.playerMask))
        {
            if (hit.collider.transform.root.GetComponentInChildren<Player>() is Player target && target.teamID != spawned.spawner.teamID)
            {
                target.data.healthHandler.CallTakeDamage(transform.up * GetScaledDamage(), target.transform.position, damagingPlayer: spawned.spawner);
            }
        }
    }
    
    private float GetScaledDamage()
    {
        var cooldown = Mathf.Max(1f, spawned.spawner.data.block.Cooldown());
        var blockCount = Mathf.Max(1f, spawned.spawner.data.block.additionalBlocks);
        return damage * (cooldown / 4f) / blockCount;
    }
}
