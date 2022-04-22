using Another_Archery_Patcher.ConfigHelpers.RecordEditor;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Noggog;
using System;
using System.Collections.Generic;

namespace Another_Archery_Patcher.ConfigHelpers
{
    using static ProjectileFlag.State;
    using static ProjectileFlag.Flag;
    using static SoundLevel;
    /// <summary>
    /// Settings object that contains all of the stats applied to <b>PROJ</b> <i>(<see cref="Projectile"/>)</i> and <b>AMMO</b> <i>(<see cref="Ammunition"/>)</i> records.
    /// </summary>
    public class StatsPresets
    {
        [MaintainOrder]
        [SettingName("Projectile Categories"), Tooltip("Change Projectile Stats Per-Category.")]
        public List<Stats> ProjectileCategories = new()
        {
            new Stats("Default",
                0,
                5000.0f,
                0.34f,
                0.44f,
                Silent
            ),
            new Stats("Arrows",
                1,
                5000.0f,
                0.34f,
                0.44f,
                Silent,
                new() { "Arrow" }
            ),
            new Stats("Bloodcursed Arrows",
                2,
                5000.0f,
                0f,
                0.44f,
                Silent,
                new(){ "DCL1ArrowElvenBloodProjectile", "DLC1AurielsBloodDippedProjectile" }
            ),
            new Stats("Bolts",
                1,
                5900.0f,
                0.34f,
                0.64f,
                Normal,
                new() { "Bolt" }
            ),
            new Stats("Explosive Bolts",
                1,
                5900.0f,
                0.34f,
                2f,
                Loud,
                new() { "Bolt" }
            ),
            new Stats("Spears",
                1,
                2800.0f,
                0.13f,
                1.1f,
                Silent,
                new() { "Riekling", "SSM", "Throw" }
            ),
            new Stats("Dart Traps",
                2,
                2000.0f,
                0.07f,
                0.2f,
                Normal,
                new() { "TrapDart" },
                new() { new(DisableCombatAimCorrection, Remove) }
            ),
            new Stats("Ballista Traps",
                3,
                6200.0f,
                0.69f,
                75.0f,
                VeryLoud,
                new() { "TrapDweBallista" },
                new() { new(DisableCombatAimCorrection, Remove) }
            )
        };

        [SettingName("Arrow Ammo Tweaks")]
        [Tooltip("These are global changes made to all Arrow ammunition records.")]
        public AmmoStats AmmoTweaksArrow = new();

        [SettingName("Bolt Ammo Tweaks")]
        [Tooltip("These are global changes made to all Bolt ammunition records.")]
        public AmmoStats AmmoTweaksBolt = new();

        /// <summary>
        /// Get the highest-priority applicable <see cref="Stats"/> category for a given projectile Editor ID.
        /// </summary>
        /// <param name="edid">The Editor ID of a <see cref="Projectile"/> record.</param>
        /// <returns>The highest-priority <see cref="Stats?"/> instance for the given editor ID, or <see cref="null"/> when no applicable categories were found.</returns>
        protected Stats? GetHighestPriorityStats(string edid) // Retrieve the highest-priority applicable category.
        {
            Stats? highestStats = null;
            var highestPriority = -1;

            ProjectileCategories.ForEach(delegate (Stats stats)
            {
                var priority = stats.GetPriority(edid);
                if (highestStats == null || priority > highestPriority)
                {
                    highestPriority = priority;
                    highestStats = stats;
                }
            });

            return highestStats;
        }
    }
}
