using Another_Archery_Patcher.ConfigHelpers.RecordEditor;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;

namespace Another_Archery_Patcher.ConfigHelpers
{
    using static Editor.Flag.State;
    using static Editor.Flag.Type;
    using static SoundLevel;
    public class StatsPresets
    {
        [MaintainOrder]
        [SettingName("Projectile Categories")]
        [Tooltip("Change Projectile Stats Per-Category.")]
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
                new List<string> { "Arrow" }
            ),
            new Stats("Bloodcursed Arrows",
                2,
                5000.0f,
                0f,
                0.44f,
                Silent,
                new List<string> { "DCL1ArrowElvenBloodProjectile", "DLC1AurielsBloodDippedProjectile" }
            ),
            new Stats("Bolts",
                1,
                5900.0f,
                0.34f,
                0.64f,
                Normal,
                new List<string> { "Bolt" }
            ),
            new Stats("Explosive Bolts",
                1,
                5900.0f,
                0.34f,
                2f,
                Loud,
                new List<string> { "Bolt" }
            ),
            new Stats("Spears",
                1,
                2800.0f,
                0.13f,
                1.1f,
                Silent,
                new List<string> { "Riekling", "SSM", "Throw" }
            ),
            new Stats("Dart Traps",
                2,
                2000.0f,
                0.07f,
                0.2f,
                Normal,
                new List<string> { "TrapDart" },
                new List<FlagTweak>() { new(DisableCombatAimCorrection, Remove) }
            ),
            new Stats("Ballista Traps",
                3,
                6200.0f,
                0.69f,
                75.0f,
                VeryLoud,
                new List<string> { "TrapDweBallista" },
                new List<FlagTweak>() { new(DisableCombatAimCorrection, Remove) }
            )
        };

        [SettingName("Arrow Ammo Tweaks")]
        [Tooltip("These are global changes made to all Arrow ammunition records.")]
        public AmmoStats AmmoTweaksArrow = new();

        [SettingName("Bolt Ammo Tweaks")]
        [Tooltip("These are global changes made to all Bolt ammunition records.")]
        public AmmoStats AmmoTweaksBolt = new();

        protected Stats? GetHighestPriorityStats(Projectile proj) // Retrieve the highest-priority applicable category.
        {
            string id = proj.EditorID!;
            if (id == null)
                throw new Exception("Attempted to get stats for null projectile!");
            Stats? highestStats = null;
            var highestPriority = -1;
            foreach (var match in ProjectileCategories)
            {
                var matchPriority = match.GetPriority(id);
                if (highestStats != null && matchPriority <= highestPriority) continue;
                highestStats = match;
                highestPriority = matchPriority;
            }
            return highestStats;
        }
    }
}
