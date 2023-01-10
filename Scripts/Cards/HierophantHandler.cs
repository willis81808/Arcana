using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnboundLib;
using EventDispatcher;
using ModsPlus;
using UnboundLib.GameModes;

public class HierophantHandler : PlayerHook
{
    [SerializeField]
    private Color cooldownIndicatorColor;

    [SerializeField]
    private float baseCooldown;
    private float cooldownCounter;

    private float cooldown
    {
        get
        {
            var attackSpeedScalar = (float)player.data.weaponHandler.gun.GetPropertyValue("usedCooldown") / 0.3f;
            return baseCooldown * Mathf.Clamp(attackSpeedScalar, 0.6f, 1.4f);
        }
    }

    [SerializeField]
    private GameObject visualsToSpawn;

    private CustomHealthBar cooldownIndicator;

    protected override void Start()
    {
        base.Start();

        cooldownIndicator = new GameObject("Hierophant Cooldown", typeof(CustomHealthBar)).GetComponent<CustomHealthBar>();
        cooldownIndicator.SetColor(cooldownIndicatorColor);
        cooldownIndicator.SetValues(cooldown, cooldown);

        player.AddStatusIndicator(cooldownIndicator.gameObject);

        HierophantHit.RegisterListener(OnHit);
    }

    private void Update()
    {
        cooldownCounter += TimeHandler.deltaTime;
        cooldownIndicator.CurrentHealth = cooldownCounter;
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();

        Destroy(cooldownIndicator.gameObject);
        HierophantHit.UnregisterListener(OnHit);
    }

    private void OnHit(HierophantHit e)
    {
        if (e.owner != player || !IsEffectReady()) return;
        
        // reset cooldown
        cooldownCounter = 0;
        cooldownIndicator.SetValues(0, cooldown);

        // spawn effect
        e.target.data.stunHandler.AddStun(3f);
        Instantiate(visualsToSpawn, e.target.transform.position, visualsToSpawn.transform.rotation);
    }

    public bool IsEffectReady()
    {
        return cooldownCounter >= cooldown;
    }

    public override IEnumerator OnBattleStart(IGameModeHandler gameModeHandler)
    {
        cooldownCounter = cooldown;
        cooldownIndicator.SetValues(cooldown, cooldown);
        yield break;
    }
}
