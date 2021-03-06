﻿// Copyright (C) 2011-2018 Bossland GmbH
// See the file LICENSE for the source code's detailed license

using Buddy.BehaviorTree;
using DefaultCombat.Core;
using DefaultCombat.Helpers;

namespace DefaultCombat.Routines
{
    public class Combat : RotationBase
    {
        public override string Name
        {
            get { return "Sentinel Combat"; }
        }

        public override Composite Buffs
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Force Might")
                    );
            }
        }

        public override Composite Cooldowns
        {
            get
            {
                return new PrioritySelector(
                    Spell.Buff("Resolute", ret => Me.IsStunned),
                    Spell.Buff("Rebuke", ret => Me.HealthPercent <= 75),
                    Spell.Buff("Force Camouflage", ret => Me.HealthPercent <= 50),
                    Spell.Buff("Saber Ward", ret => Me.HealthPercent <= 25),
                    Spell.Buff("Guarded by the Force", ret => Me.HealthPercent <= 15),
                    Spell.Buff("Valorous Call", ret => Me.BuffCount("Centering") < 15),
                    Spell.Buff("Zen"),
                    Spell.Cast("Unity", ret => Me.HealthPercent <= 15)
                    );
            }
        }

        public override Composite SingleTarget
        {
            get
            {
                return new PrioritySelector(
                    Spell.Cast("Twin Saber Throw", ret => CombatHotkeys.EnableCharge && !DefaultCombat.MovementDisabled && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),
                    Spell.Cast("Force Leap", ret => CombatHotkeys.EnableCharge && Me.CurrentTarget.Distance >= 1f && Me.CurrentTarget.Distance <= 3f),

                    //Movement
                    CombatMovement.CloseDistance(Distance.Melee),

                    //Legacy Heroic Moment Abilities --will only be active when user initiates Heroic Moment--
                    Spell.Cast("Legacy Force Sweep", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.5f),
                    Spell.CastOnGround("Legacy Orbital Strike", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Project", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Dirty Kick", ret => Me.HasBuff("Heroic Moment") && Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Legacy Sticky Plasma Grenade", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Flame Thrower", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Lightning", ret => Me.HasBuff("Heroic Moment")),
                    Spell.Cast("Legacy Force Choke", ret => Me.HasBuff("Heroic Moment")),

                    //Rotation
                    Spell.Cast("Force Kick", ret => Me.CurrentTarget.IsCasting && CombatHotkeys.EnableInterrupts),
                    Spell.Cast("Dispatch", ret => Me.HasBuff("Hand of Justice") || Me.CurrentTarget.HealthPercent <= 30),
                    Spell.Cast("Precision", ret => Me.CurrentTarget.Distance <= 0.4f),
                    Spell.Cast("Blade Barrage", ret => Me.HasBuff("Precision")),
                    Spell.Cast("Clashing Blast", ret => Me.HasBuff("Opportune Attack")),
                    Spell.Cast("Lance"),
                    Spell.Cast("Blade Storm", ret => Me.HasBuff("Opportune Attack") && Me.Level < 57),
                    Spell.Cast("Blade Rush"),
                    Spell.Cast("Slash", ret => Me.ActionPoints >= 7 && Me.Level < 26),
                    Spell.Cast("Zealous Strike", ret => Me.ActionPoints <= 7),
                    Spell.Cast("Strike", ret => Me.ActionPoints <= 10)
                    );
            }
        }

        public override Composite AreaOfEffect
        {
            get
            {
                return new Decorator(ret => Targeting.ShouldPbaoe,
                    new PrioritySelector(
                        Spell.Cast("Precision"),
                        Spell.Cast("Cyclone Slash"),
                        Spell.Cast("Force Sweep"),
                        Spell.Cast("Dispatch", ret => Me.HasBuff("Hand of Justice") || Me.CurrentTarget.HealthPercent <= 30),
                        Spell.Cast("Zealous Strike", ret => Me.ActionPoints <= 7)
                        ));
            }
        }
    }
}
