using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;

namespace AATPatcher
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "YourPatcher.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            foreach (var proj in state.LoadOrder.PriorityOrder.Projectile().WinningOverrides()) {
                var id = proj.EditorID;
                if ( id != null )
                {
                    if ( id.Contains("Arrow", StringComparison.OrdinalIgnoreCase) ) // if projectile is an arrow
                    {
                        var arrow = state.PatchMod.Projectiles.GetOrAddAsOverride(proj);
                        // handle arrow
                    }
                    else if ( id.Contains("Bolt", StringComparison.OrdinalIgnoreCase) ) // if projectile is a bolt
                    {
                        var bolt = state.PatchMod.Projectiles.GetOrAddAsOverride(proj);
                        // handle bolt
                    }
                    else if ( proj.Type == Projectile.TypeEnum.Arrow ) // if projectile is at least of type arrow
                    {
                        var special = state.PatchMod.Projectiles.GetOrAddAsOverride(proj);
                        // print log message
                    }
                }
            }
        }
    }
}
