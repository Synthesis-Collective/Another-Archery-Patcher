using System;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace Another_Archery_Patcher
{
    public class Program
    {
        /**
         * @brief Handles modifying a single projectile record.
         * @param state - Current patcher state instance.
         * @param proj  - Target projectile record.
         * @param stats - Projectile stats to apply.
         * @returns int
         *\n        0   - Failed to apply stats to projectile because an exception was thrown.
         *\n        1   - Successfully added modified projectile to patch.
         */
        private static bool HandleProjectile(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, IProjectileGetter proj, string id, ProjectileStats stats)
        {
            try
            {
                var projectile = state.PatchMod.Projectiles.GetOrAddAsOverride(proj);
                projectile.Speed = stats.Speed;
                projectile.Gravity = stats.Gravity;
                projectile.ImpactForce = stats.ImpactForce;
                projectile.SoundLevel = (uint)stats.SoundLevel;
                if (Settings.MiscTweaks.DisableSupersonic && (projectile.Flags & Projectile.Flag.Supersonic) != 0)
                    projectile.Flags &= ~Projectile.Flag.Supersonic;
                Console.WriteLine("[LOG]\tFinished processing projectile: \"" + id + "\"");
                return true;
            }
            catch (Exception ex) // log any exceptions
            {
                Console.WriteLine("[ERROR]\t\"" + ex.Message + "\" occurred while processing \"" + id + "\"");
                return false;
            }
        }

        private static bool HandleTrap(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, IProjectileGetter proj, string id)
        {
            // intercept ballista traps if enabled
            if (Settings.MiscTweaks.PatchTraps && id.Contains("TrapDweBallista", StringComparison.OrdinalIgnoreCase))
                return HandleProjectile(state, proj, id, new ProjectileStats(6400.0f, 0.69f, 75.0f, SoundLevel.VeryLoud));
            // intercept other traps if enabled
            if (Settings.MiscTweaks.PatchTraps && id.Contains("Trap", StringComparison.OrdinalIgnoreCase))
                return HandleProjectile(state, proj, id, new ProjectileStats(3000.0f, 0.0f, 0.2f, SoundLevel.Normal));
            return false;
        }
        // Handle Special Projectiles ( Currently only Bloodcursed Arrows )
        private static bool HandleSpecial(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, IProjectileGetter proj, string id, ProjectileStats stats)
        {
            // intercept bloodcursed arrows if enabled
            if (Settings.MiscTweaks.DisableGravityBloodcursed && Settings.MiscTweaks.BloodcursedId.Contains(id, StringComparer.OrdinalIgnoreCase))
                return HandleProjectile(state, proj, id, new ProjectileStats(stats.Speed, 0.0f, stats.ImpactForce, stats.SoundLevel));
            return false;
        }

        private static bool HandleRecord(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, IProjectileGetter proj, string id)
        {
            if ( HandleTrap(state, proj, id) )
                return true;
            // else check for perfect matches (all matches satisfied)
            foreach (var tweak in Settings.ProjectileTweaks.Where(tweak => tweak.IsPerfectMatch(id)))
                return HandleSpecial(state, proj, id, tweak.Stats) || HandleProjectile(state, proj, id, tweak.Stats);
            // else check for any matches
            return Settings.ProjectileTweaks.Where(tweak => tweak.IsMatch(id)).Select(tweak => HandleSpecial(state, proj, id, tweak.Stats) || HandleProjectile(state, proj, id, tweak.Stats)).FirstOrDefault();
        }
        /**
         * @brief Checks if a given projectile is not on any blacklist, and is a valid target.
         * @param proj      - The projectile to check.
         * @param editorID  - A string var to assign to the editor ID of the given projectile.
         * @returns bool
         *\n        true    - Projectile is a valid type, and is not on the blacklist.
         *\n        false   - Projectile is not a valid target, skip it.
         */
        private static bool IsValidPatchTarget(IProjectileGetter proj, out string editorId)
        {
            if (proj.EditorID != null) { // Editor ID is valid, check if projectile type is valid & projectile isn't present on any blacklist.
                editorId = proj.EditorID;
                // Return true if: type is Arrow and is not blacklisted OR if the patch_traps option is enabled, type is missile, editor ID contains "trap", and is not blacklisted
                return ( proj.Type == Projectile.TypeEnum.Arrow && !Settings.Blacklist.IsMatch(editorId)) || ( Settings.MiscTweaks.PatchTraps && proj.Type == Projectile.TypeEnum.Missile && proj.EditorID.Contains("Trap", StringComparison.OrdinalIgnoreCase) && !Settings.Blacklist.IsMatch(editorId) );
            }
            editorId = "";
            return false;
        }

        private static Lazy<TopLevelSettings> _lazySettings = new();
        private static TopLevelSettings Settings => _lazySettings.Value; // convenience wrapper

        public static async Task<int> Main(string[] args) {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings("Settings", "settings.json", out _lazySettings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "AnotherArcheryPatcher.esp")
                .Run(args);
        }
        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if ( Settings == null )
                throw new Exception("Settings were null, something went very wrong during initialization!"); // throw early if settings are null

            if (Settings.UseVerboseLog) { // print out the current settings
                Console.WriteLine("\n--- CONFIGURATION ---");
                Console.WriteLine("Keep Auto-Aim:\t\t" + !Settings.GameSettings.DisableAutoaim);
                Console.WriteLine("Keep Ninja Dodge:\t\t" + !Settings.GameSettings.DisableNpcDodge);
                Console.WriteLine("Keep Supersonic:\t\t" + !Settings.MiscTweaks.DisableSupersonic);
                Console.WriteLine("Keep Vanilla Traps:\t\t" + !Settings.MiscTweaks.PatchTraps);

                if (Settings.ProjectileTweaks.Any()) { // print out projectile tweak categories
                    Console.Write("Projectile Tweaks:{");
                    foreach (var tweak in Settings.ProjectileTweaks)
                        Console.Write('\n' + tweak.GetVarsAsString());
                    Console.WriteLine("}");
                }

                if (Settings.Blacklist.Enabled) { // print out blacklisted projectiles
                    Console.Write("Blacklist: {");
                    foreach (var id in Settings.Blacklist.Matchlist)
                        Console.Write("\n\t" + id.Name + ( id.Required ? "[!]" : "" ));
                    foreach (var id in Settings.Blacklist.Record)
                        Console.Write("\n\t" + id);
                    Console.WriteLine("\n}");
                }
            }

            Console.WriteLine("\n--- BEGIN PATCHER PROCESS ---"); // begin

            // Handle Game Settings
            if ( Settings.GameSettings.DisableAutoaim ) { // remove auto-aim
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fAutoAimMaxDegrees", Data = 0.0f });          // Add new game setting to patch: "fAutoAimMaxDegrees"
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fAutoAimMaxDistance", Data = 0.0f });         // Add new game setting to patch: "fAutoAimMaxDistance"
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fAutoAimScreenPercentage", Data = 0.0f });    // Add new game setting to patch: "fAutoAimScreenPercentage"
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fAutoAimMaxDegrees3rdPerson", Data = 0.0f }); // Add new game setting to patch: "fAutoAimMaxDegrees3rdPerson"
                Console.WriteLine("Finished removing auto-aim.");
            }
            if ( Settings.GameSettings.DisableNpcDodge ) { // disable npc ninja-dodge
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fCombatDodgeChanceMax", Data = 0.0f });       // Add new game setting to patch: "fCombatDodgeChanceMax"
                Console.WriteLine("Finished patching NPC Ninja Dodge bug.");
            }

            // Handle Projectiles
            var count = 0;
            foreach ( var proj in state.LoadOrder.PriorityOrder.Projectile().WinningOverrides() )
                if (IsValidPatchTarget(proj, out string id))
                    count += HandleRecord(state, proj, id) ? 1 : 0;

            Console.WriteLine("--- END PATCHER PROCESS ---\nProcessed " + count + " projectile records successfully.\n"); // end
        }
    }
}
