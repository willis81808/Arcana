using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using UnboundLib.Extensions;
using ModsPlus;
using ModdingUtils.Utils;

public class ThornTrail : PlayerHook
{
    [SerializeField]
    private ParticleSystem trail;

    [SerializeField]
    private ThornsZone zone;
    
    private float zoneSpawnCooldown = 0;
    private const float zoneSpawnInterval = 0.25f;

    private float timeSinceEnabled;

    private void Update()
    {
        var emitter = trail.emission;

        if (!PlayerStatus.PlayerAliveAndSimulated(player) || !player.IsFullyAlive())
        {
            timeSinceEnabled = Time.time;
            emitter.enabled = false;
            return;
        }

        if (Time.time - timeSinceEnabled <= 1)
        {
            return;
        }

        var floorMask = LayerMask.GetMask(new string[] { "Default", "IgnorePlayer" });
        var hit = Physics2D.Raycast(transform.position, -transform.up, 5f, floorMask);
        if (hit.collider == null)
        {
            emitter.enabled = false;
        }
        else if (!emitter.enabled)
        {
            emitter.enabled = true;
        }
        else
        {
            trail.transform.position = hit.point;
            trail.transform.up = hit.normal;
        }
    }

    private void FixedUpdate()
    {
        zoneSpawnCooldown += Time.fixedDeltaTime;

        if (!trail.emission.enabled) return;

        if (zoneSpawnCooldown >= zoneSpawnInterval)
        {
            zoneSpawnCooldown = 0;
            SpawnNewZone();
        }
    }
    
    private void SpawnNewZone()
    {
        Instantiate(zone, trail.transform.position, trail.transform.rotation).Initialize(player);
    }
}
