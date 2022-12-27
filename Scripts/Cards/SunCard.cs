using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ModsPlus;

public class SunCard : CustomEffectCard<SunHandler>
{
    public override CardDetails Details => new CardDetails
    {
        Title = "The Sun",
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
    {
        cardInfo.allowMultiple = false;
    }
}

public class SunHandler : CardEffect
{
    /*
    private bool hidden = false;

    private List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    private List<ParticleSystem> particles = new List<ParticleSystem>();

    public override void OnBlock(BlockTrigger.BlockTriggerType blockTriggerType)
    {
        if (hidden)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (var sprite in sprites)
        {
            sprite.enabled = true;
        }
        foreach (var particle in particles)
        {
            particle.gameObject.SetActive(true);
        }

        sprites.Clear();
        particles.Clear();

        hidden = false;
    }

    private void Hide()
    {
        foreach (var sprite in GetComponentsInChildren<SpriteRenderer>())
        {
            if (!sprite.enabled) continue;
            sprite.enabled = false;
            sprites.Add(sprite);
        }
        foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        {
            if (!particle.gameObject.activeInHierarchy) continue;
            particle.gameObject.SetActive(false);
            particles.Add(particle);
        }
        hidden = true;
    }
    */
}
