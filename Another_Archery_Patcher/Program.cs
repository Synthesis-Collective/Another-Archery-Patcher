using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Mutagen.Bethesda.Plugins;

namespace Another_Archery_Patcher
{
    public class Program
    {
        // SETTINGS
        public enum SoundLevel // todo: figure out what integers are used for sound levels
        {
            [MaintainOrder]
            silent = 2,
            normal = 1,
            loud = 0,
            very_loud = 3,
        }
        public class GameSettings // game setting settings
        {
            [MaintainOrder]
            [Tooltip("Disable Auto-Aim")]
            public bool disable_autoaim = true;
            [Tooltip("[Experimental] Disable NPC Dodge to prevent the \"ninja dodge\" bug. May interfere with some combat mods.")]
            public bool disable_npcDodge = false;
            public GameSettings(bool disableAutoAim, bool disableNPCDodge)
            {
                disable_autoaim = disableAutoAim;
                disable_npcDodge = disableNPCDodge;
            }
        }
        public class GeneralTweaks // global projectile tweaks
        {
            [MaintainOrder]
            [Tooltip("Remove the supersonic flag from projectiles of this type. The supersonic flag removes sound from in-flight projectiles.")]
            public bool disable_supersonic;
            public GeneralTweaks(bool disableSupersonicFlag)
            {
                disable_supersonic = disableSupersonicFlag;
            }
        }
        public class ProjectileTweaks // projectile tweaks
        {
            [MaintainOrder]
            [Tooltip("Toggle the tweaks in this section.")]
            public bool enabled;
            [Tooltip("The speed of this type of projectile. Controls projectile drop.")]
            public float speed;
            [Tooltip("The amount of gravity applied to this type of projectile. Controls projectile drop.")]
            public float gravity;
            [Tooltip("The amount of force imparted into objects hit by projectiles of this type.")]
            public float impactForce;
            [Tooltip("The amount of detectable noise produced by in-flight projectiles.")]
            public SoundLevel soundLevel;
            public ProjectileTweaks(bool enable, float proj_speed, float proj_gravity, float proj_impactForce, SoundLevel proj_soundLevel)
            {
                enabled = enable;
                speed = proj_speed;
                gravity = proj_gravity;
                impactForce = proj_impactForce;
                soundLevel = proj_soundLevel;
            }
        }
        public class ProjectileTweaks_Arrow : ProjectileTweaks // arrow-specific projectile tweak specialization
        {
            [Tooltip("Makes it easier/possible to hit the sun with the Dawnguard DLC's Bloodcursed Elven Arrows"), SettingName("Remove Gravity from Bloodcursed Elven Arrows")]
            public bool disable_gravity_bloodcursed = true;
            [Ignore]
            public string[] bloodcursed_id = { "DCL1ArrowElvenBloodProjectile", "DLC1AurielsBloodDippedProjectile" }; // Editor ID list of bloodcursed arrows
            public ProjectileTweaks_Arrow(bool enable, float proj_speed, float proj_gravity, float proj_impactForce, SoundLevel proj_soundLevel, bool bloodcursed_fix) : base(enable, proj_speed, proj_gravity, proj_impactForce, proj_soundLevel)
            {
                disable_gravity_bloodcursed = bloodcursed_fix;
            }
        }
        public class TopLevelSettings // main settings object
        {
            [MaintainOrder]
            [Tooltip("Changes Game Settings. (GMST)"), SettingName("[GMST] Game Settings")]
            public GameSettings GameSettings = new(true, false);
            [Tooltip("Tweaks that are applied to all projectiles."), SettingName("Universal Projectile Tweaks")]
            public GeneralTweaks GeneralTweaks = new(true);
            [Tooltip("Tweaks that are applied to Arrows."), SettingName("Arrow Tweaks")]
            public ProjectileTweaks_Arrow ArrowTweaks = new(true, 5000.0f, 0.34f, 0.44f, SoundLevel.silent, true);
            [Tooltip("Tweaks that are applied to Bolts."), SettingName("Bolt Tweaks")]
            public ProjectileTweaks BoltTweaks = new(true, 5800.0f, 0.34f, 0.64f, SoundLevel.normal);
            [Tooltip("Tweaks that are applied to Throwable Weapons & Spears."), SettingName("Throwable Tweaks")]
            public ProjectileTweaks ThrowableTweaks = new(true, 2800.0f, 0.13f, 1.1f, SoundLevel.silent);
            [Tooltip("Projectiles in this list will be skipped without changing anything."), SettingName("Projectile Blacklist")]
            public List<IFormLinkGetter<IProjectileGetter>> blacklist = new();
        }
        static Lazy<TopLevelSettings> settings = null!;
        // MAIN
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings( "Settings", "settings.json", out settings )
                .SetTypicalOpen(GameRelease.SkyrimSE, "AnotherArcheryPatcher.esp")
                .Run(args);
        }
        // FUNCTIONS
        public static void HandleProjectile(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, Mutagen.Bethesda.Skyrim.IProjectileGetter proj, ProjectileTweaks tweaks)
        {
            var projectile = state.PatchMod.Projectiles.GetOrAddAsOverride(proj);
            projectile.Speed = tweaks.speed;
            projectile.Gravity = tweaks.gravity;
            projectile.ImpactForce = tweaks.impactForce;
            projectile.SoundLevel = (uint)tweaks.soundLevel;
            if (projectile.Flags.HasFlag(Projectile.Flag.Supersonic) && settings.Value.GeneralTweaks.disable_supersonic) { projectile.Flags &= ~Projectile.Flag.Supersonic; }
        }
        public static bool IsBloodcursedArrow(string editorID)
        {
            foreach (string it in settings.Value.ArrowTweaks.bloodcursed_id)
                if (it == editorID)
                    return true;
            return false;

        }
        public static bool IsBlacklisted(Mutagen.Bethesda.Skyrim.IProjectileGetter proj)
        {
            return settings.Value.blacklist.Contains(proj) || proj.EditorID == "MQ101ArrowSteelProjectile"; // check if the blacklist contains projectile
        }
        // PATCHER FUNCTION
        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if ( settings == null ) throw new Exception("Settings were null! (How did this happen?)"); // throw early if settings are null
            if (settings.Value.GameSettings.disable_autoaim) // disable auto-aim
            {
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fAutoAimMaxDegrees", Data = 0.0f });          // Add new game setting to patch: "fAutoAimMaxDegrees"
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fAutoAimMaxDistance", Data = 0.0f });         // Add new game setting to patch: "fAutoAimMaxDistance"
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fAutoAimScreenPercentage", Data = 0.0f });    // Add new game setting to patch: "fAutoAimScreenPercentage"
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fAutoAimMaxDegrees3rdPerson", Data = 0.0f }); // Add new game setting to patch: "fAutoAimMaxDegrees3rdPerson"
            }
            if (settings.Value.GameSettings.disable_npcDodge) // disable ninja dodge
            {
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fCombatDodgeChanceMax", Data = 0.0f });       // Add new game setting to patch: "fCombatDodgeChanceMax"
            }
            foreach (var proj in state.LoadOrder.PriorityOrder.Projectile().WinningOverrides()) { // iterate through winning projectile overrides
                if (!IsBlacklisted(proj))
                {
                    var id = proj.EditorID; // get the editor ID of this projectile
                    if (id != null && proj.Type == Projectile.TypeEnum.Arrow) // valid editor ID
                    {
                        if (IsBloodcursedArrow(id)) // check id against special projectiles
                        {
                            if (settings.Value.ArrowTweaks.disable_gravity_bloodcursed)
                            {
                                HandleProjectile(state, proj, new(true, settings.Value.ArrowTweaks.speed, 0.0f, settings.Value.ArrowTweaks.impactForce, settings.Value.ArrowTweaks.soundLevel));
                                Console.WriteLine("Finished processing special arrow: \"" + id + "\" (Disabled Gravity)");
                            }
                            else
                            {
                                HandleProjectile(state, proj, settings.Value.ArrowTweaks);
                                Console.WriteLine("Finished processing arrow: \"" + id + '\"');
                            }
                        }
                        else if (id.Contains("Trap", StringComparison.OrdinalIgnoreCase) && settings.Value.BoltTweaks.enabled)
                        {
                            HandleProjectile(state, proj, new(true, 6400.0f, 0.69f, 75.0f, SoundLevel.very_loud));
                            Console.WriteLine("Finished processing bolt: \"" + id + '\"');
                        }
                        else if ((id.Contains("SSM", StringComparison.OrdinalIgnoreCase) || id.Contains("Spear", StringComparison.OrdinalIgnoreCase)) && settings.Value.ThrowableTweaks.enabled)
                        { // if projectile is a spear/throwable
                            HandleProjectile(state, proj, settings.Value.ThrowableTweaks);
                            Console.WriteLine("Finished processing spear: \"" + id + '\"');
                        } // else continue
                        else if (id.Contains("Arrow", StringComparison.OrdinalIgnoreCase) && settings.Value.ArrowTweaks.enabled)
                        { // if projectile is an arrow
                            HandleProjectile(state, proj, settings.Value.ArrowTweaks);
                            Console.WriteLine("Finished processing arrow: \"" + id + '\"');
                        } // else continue
                        else if (id.Contains("Bolt", StringComparison.OrdinalIgnoreCase) && settings.Value.BoltTweaks.enabled)
                        { // if projectile is a bolt
                            HandleProjectile(state, proj, settings.Value.BoltTweaks);
                            Console.WriteLine("Finished processing bolt: \"" + id + '\"');
                        } // else continue
                    }
                }
            }
        }
    }
}
