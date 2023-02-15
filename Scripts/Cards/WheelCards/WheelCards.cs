using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnboundLib;
using ModsPlus;
using System.Collections;

public class HealthOne : SimpleCard
{
    public override CardDetails Details => new CardDetails
    {
        Title = "Health - I",
        Description = "<color=\"purple\">[ Fortune ]</color>",
        Stats = new CardInfoStat[]
        {
            new CardInfoStat
            {
                positive = true,
                amount = "+25%",
                simepleAmount = CardInfoStat.SimpleAmount.Some,
                stat = "Health"
            },
            new CardInfoStat
            {
                positive = false,
                amount = "-10%",
                simepleAmount = CardInfoStat.SimpleAmount.lower,
                stat = "Damage"
            }
        },
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
    {
        statModifiers.health = 1.25f;
        gun.damage = 0.9f;
    }

    public override bool GetEnabled()
    {
        return false;
    }
}

public class HealthTwo : SimpleCard
{
    public override CardDetails Details => new CardDetails
    {
        Title = "Health - II",
        Description = "<color=\"purple\">[ Fortune ]</color>",
        Stats = new CardInfoStat[]
        {
            new CardInfoStat
            {
                positive = true,
                amount = "+50%",
                simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf,
                stat = "Health"
            },
            new CardInfoStat
            {
                positive = false,
                amount = "-20%",
                simepleAmount = CardInfoStat.SimpleAmount.lower,
                stat = "Damage"
            }
        },
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
    {
        statModifiers.health = 1.5f;
        gun.damage = 0.8f;
    }

    public override bool GetEnabled()
    {
        return false;
    }
}

public class DamageOne: SimpleCard
{
    public override CardDetails Details => new CardDetails
    {
        Title = "Damage - I",
        Description = "<color=\"purple\">[ Fortune ]</color>",
        Stats = new CardInfoStat[]
        {
            new CardInfoStat
            {
                positive = true,
                amount = "+25%",
                simepleAmount = CardInfoStat.SimpleAmount.Some,
                stat = "Damage"
            },
            new CardInfoStat
            {
                positive = false,
                amount = "-10%",
                simepleAmount = CardInfoStat.SimpleAmount.lower,
                stat = "Health"
            }
        },
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
    {
        gun.damage = 1.25f;
        statModifiers.health = 0.9f;
    }

    public override bool GetEnabled()
    {
        return false;
    }
}

public class DamageTwo : SimpleCard
{
    public override CardDetails Details => new CardDetails
    {
        Title = "Damage - II",
        Description = "<color=\"purple\">[ Fortune ]</color>",
        Stats = new CardInfoStat[]
        {
            new CardInfoStat
            {
                positive = true,
                amount = "+50%",
                simepleAmount = CardInfoStat.SimpleAmount.aHugeAmountOf,
                stat = "Damage"
            },
            new CardInfoStat
            {
                positive = false,
                amount = "-20%",
                simepleAmount = CardInfoStat.SimpleAmount.lower,
                stat = "Health"
            }
        },
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers)
    {
        gun.damage = 1.5f;
        statModifiers.health = 0.8f;
    }

    public override bool GetEnabled()
    {
        return false;
    }
}

public class ReloadOne : SimpleCard
{
    public override CardDetails Details => new CardDetails
    {
        Title = "Reload - I",
        Description = "<color=\"purple\">[ Fortune ]</color>",
        Stats = new CardInfoStat[]
        {
            new CardInfoStat
            {
                positive = true,
                amount = "-25%",
                simepleAmount = CardInfoStat.SimpleAmount.Some,
                stat = "Reload Time"
            },
            new CardInfoStat
            {
                positive = false,
                amount = "+10%",
                simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf,
                stat = "Block Cooldown"
            }
        },
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
    {
        gun.reloadTime = 0.75f;
        block.cdMultiplier = 1.1f;
    }

    public override bool GetEnabled()
    {
        return false;
    }
}

public class ReloadTwo : SimpleCard
{
    public override CardDetails Details => new CardDetails
    {
        Title = "Reload - II",
        Description = "<color=\"purple\">[ Fortune ]</color>",
        Stats = new CardInfoStat[]
        {
            new CardInfoStat
            {
                positive = true,
                amount = "-50%",
                simepleAmount = CardInfoStat.SimpleAmount.Some,
                stat = "Reload Time"
            },
            new CardInfoStat
            {
                positive = false,
                amount = "+20%",
                simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf,
                stat = "Block Cooldown"
            }
        },
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
    {
        gun.reloadTime = 0.5f;
        block.cdMultiplier = 1.2f;
    }

    public override bool GetEnabled()
    {
        return false;
    }
}

public class BlockOne : SimpleCard
{
    public override CardDetails Details => new CardDetails
    {
        Title = "Block - I",
        Description = "<color=\"purple\">[ Fortune ]</color>",
        Stats = new CardInfoStat[]
        {
            new CardInfoStat
            {
                positive = true,
                amount = "-25%",
                simepleAmount = CardInfoStat.SimpleAmount.Some,
                stat = "Block Cooldown"
            },
            new CardInfoStat
            {
                positive = false,
                amount = "+10%",
                simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf,
                stat = "Reload Time"
            }
        },
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
    {
        block.cdMultiplier = 0.75f;
        gun.reloadTime = 1.1f;
    }
    
    public override bool GetEnabled()
    {
        return false;
    }
}

public class BlockTwo : SimpleCard
{
    public override CardDetails Details => new CardDetails
    {
        Title = "Block - II",
        Description = "<color=\"purple\">[ Fortune ]</color>",
        Stats = new CardInfoStat[]
        {
            new CardInfoStat
            {
                positive = true,
                amount = "-50%",
                simepleAmount = CardInfoStat.SimpleAmount.Some,
                stat = "Block Cooldown"
            },
            new CardInfoStat
            {
                positive = false,
                amount = "+20%",
                simepleAmount = CardInfoStat.SimpleAmount.aLittleBitOf,
                stat = "Reload Time"
            }
        },
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
    {
        block.cdMultiplier = 0.5f;
        gun.reloadTime = 1.2f;
    }

    public override bool GetEnabled()
    {
        return false;
    }
}


public class AmmoOne : SimpleCard
{
    public override CardDetails Details => new CardDetails
    {
        Title = "Ammo - I",
        Description = "<color=\"purple\">[ Fortune ]</color>",
        Stats = new CardInfoStat[]
        {
            new CardInfoStat
            {
                positive = true,
                amount = "+3",
                simepleAmount = CardInfoStat.SimpleAmount.Some,
                stat = "Ammo"
            }
        },
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
    {
        gun.ammo = 3;
    }

    public override bool GetEnabled()
    {
        return false;
    }
}


public class AmmoTwo : SimpleCard
{
    public override CardDetails Details => new CardDetails
    {
        Title = "Ammo - II",
        Description = "<color=\"purple\">[ Fortune ]</color>",
        Stats = new CardInfoStat[]
        {
            new CardInfoStat
            {
                positive = true,
                amount = "+6",
                simepleAmount = CardInfoStat.SimpleAmount.Some,
                stat = "Ammo"
            }
        },
        ModName = "Arcana"
    };

    public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block)
    {
        gun.ammo = 6;
    }

    public override bool GetEnabled()
    {
        return false;
    }
}