using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher.ConfigHelpers
{
    using static SoundLevel;
    public class StatsDefault
    {
        [MaintainOrder]
        [Tooltip("Change Projectile Stats Per-Category.")]
        public List<Stats> ProjectileTweaks = new()
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
                new List<string>{ "Arrow" }
            ),
            new Stats("Bolts",
                1, 
                5900.0f, 
                0.34f, 
                0.64f, 
                Normal, 
                new List<string>{ "Bolt" }
            ),
            new Stats("Explosive Bolts",
                1,
                5900.0f, 
                0.34f, 
                2f, 
                Loud, 
                new List<string>{ "Bolt" }
            ),
            new Stats("Spears",
                1, 
                2800.0f, 
                0.13f, 
                1.1f, 
                Silent, 
                new List<string>{ "Riekling", "SSM", "Throw" }
            ),
            new Stats("Dart Traps",
                2, 
                2000.0f,
                0.07f,
                0.2f, 
                Normal, 
                new List<string>{ "TrapDart" }
            ),
            new Stats("Ballista Traps",
                3, 
                6200.0f, 
                0.69f, 
                75.0f, 
                VeryLoud, 
                new List<string>{ "TrapDweBallista" }
            )
        };

        protected Stats? GetHighestPriorityStats(Projectile proj) // Retrieve the highest-priority applicable category.
        {
            string id = proj.EditorID!;
            if (id == null)
                throw new Exception("Attempted to get stats for null projectile!");
            Stats? highestStats = null;
            var highestPriority = -1;
            foreach (var match in ProjectileTweaks)
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
