using UnityEngine;
using EventDispatcher;

public class HierophantHitEffect : RayHitEffect
{
    private Player owner;

    private void Start()
    {
        owner = transform.root.GetComponentInChildren<ProjectileHit>().ownPlayer;
    }

    public override HasToReturn DoHitEffect(HitInfo hit)
    {
        if (hit.collider.GetComponentInParent<Player>() is Player p)
        {
            new HierophantHit(owner, p).FireEvent();
        }
        
        return HasToReturn.canContinue;
    }
}

public class HierophantHit : Event<HierophantHit>
{
    public Player owner, target;

    public HierophantHit(Player owner, Player target)
    {
        this.owner = owner;
        this.target = target;
    }
}