using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;

namespace Another_Archery_Patcher
{
    using static SoundLevel;
    // SETTINGS
    public enum SoundLevel // this is needed because reflective settings don't work with Mutagen.Bethesda.Skyrim.SoundLevel for some reason
    {
        [MaintainOrder]
        Silent = 2,
        Normal = 1,
        Loud = 0,
        VeryLoud = 3,
    }
    public class GameSettings // game setting settings
    {
        public GameSettings(bool disableAutoAim, bool disableNPCDodge)
        {
            disable_autoaim = disableAutoAim;
            disable_npcDodge = disableNPCDodge;
        }
        [MaintainOrder]
        [SettingName("Disable Auto-Aim"), Tooltip("Removes the terrible vanilla auto-aim from 1st and 3rd person.")]
        public bool disable_autoaim = true;
        [SettingName("Patch NPC Ninja Dodge Bug"), Tooltip("Prevents NPCs from dodging your arrows at range.")]
        public bool disable_npcDodge = true;
    }
    public class MiscTweaks // misc projectile tweaks
    {
        public MiscTweaks(bool disableSupersonicFlag, bool removeBloodcursedGravity, bool patchTraps)
        {
            disable_supersonic = disableSupersonicFlag;
            disable_gravity_bloodcursed = removeBloodcursedGravity;
            patch_traps = patchTraps;
        }
        [MaintainOrder]
        [Tooltip("Remove the supersonic flag from projectiles of this type. The supersonic flag removes sound from in-flight projectiles.")]
        public bool disable_supersonic;
        [Ignore] public List<string> bloodcursed_id = new() { "DCL1ArrowElvenBloodProjectile", "DLC1AurielsBloodDippedProjectile" }; // Editor ID list of bloodcursed arrows
        [SettingName("No Gravity for Bloodcursed Arrows"), Tooltip("Makes it easier/possible to hit the sun with the Dawnguard DLC's Bloodcursed Elven Arrows")]
        public bool disable_gravity_bloodcursed;
        [SettingName("Patch Trap Projectiles"), Tooltip("Modifies most of the projectiles fired by dart traps & dwemer ballista traps to be more interesting. (If anyone wants to customize the values, let me know and I'll add it)")]
        public bool patch_traps;
    }
    public class Matchable // Base class used to indicate that an object can be matched against editor IDs
    {
        public Matchable(List<string>? matchlist, bool enabled = true)
        {
            _enabled = enabled;
            _matchlist = matchlist ?? (new());
        }
        public bool IsMatch(string? id, bool allow_partial_match = true)
        {
            if (id != null && _enabled && _matchlist.Count > 0) {
                if (!allow_partial_match)
                    return _matchlist.Contains(id);
                foreach(var comp in _matchlist)
                    if (id.Contains(comp, StringComparison.OrdinalIgnoreCase) || comp == id)
                        return true;
            }
            return false;
        }
        [MaintainOrder]
        [SettingName("Enable"), JsonDiskName("enabled")]
        public bool _enabled;
        [SettingName("Common Names"), JsonDiskName("matchlist"), Tooltip("(Don't change this unless you know what you're doing!) Used to resolve projectile type, as there is no other way to distinguish between arrows/bolts/other")]
        public List<string> _matchlist;
    }
    public class MatchableRecord : Matchable
    {
        public MatchableRecord(List<string> blacklistedIDs, List<IFormLinkGetter<IProjectileGetter>> blacklistedRecords) : base(blacklistedIDs)
        {
            record = blacklistedRecords;
        }
        [MaintainOrder]
        [SettingName("Records")]
        public List<IFormLinkGetter<IProjectileGetter>> record;
    }
    public class ProjectileStats // Contains projectile stats only. Used to maintain ordering with inheritance
    {
        public ProjectileStats(float proj_speed, float proj_gravity, float proj_impactForce, SoundLevel proj_soundLevel)
        {
            speed = proj_speed;
            gravity = proj_gravity;
            impactForce = proj_impactForce;
            soundLevel = proj_soundLevel;
        }
        [MaintainOrder]
        [SettingName("Speed"), Tooltip("The speed of this type of projectile. Controls projectile drop.")]
        public float speed;
        [SettingName("Gravity"), Tooltip("The amount of gravity applied to this type of projectile. Controls projectile drop.")]
        public float gravity;
        [SettingName("Impact Force"), Tooltip("The amount of force imparted into objects hit by projectiles of this type.")]
        public float impactForce;
        [SettingName("Sound Level"), Tooltip("The amount of detectable noise produced by in-flight projectiles.")]
        public SoundLevel soundLevel;
    }

    public class ProjectileTweaks : Matchable // Matchable projectile stats wrapper
    {
        public ProjectileTweaks(bool enable, float proj_speed, float proj_gravity, float proj_impactForce, SoundLevel proj_soundLevel, List<string>? matchable_ids = null) : base(matchable_ids, enable)
        {
            stats = new ProjectileStats(proj_speed, proj_gravity, proj_impactForce, proj_soundLevel);
        }
        public ProjectileTweaks(bool enable, ProjectileStats proj_stats, List<string>? matchable_ids = null) : base(matchable_ids, enable)
        {
            stats = proj_stats;
        }
        [MaintainOrder]
        [SettingName("Stats")]
        public ProjectileStats stats;
    }
    public class TopLevelSettings // top level settings object
    {
        [MaintainOrder]
        [SettingName("Game Setting Tweaks")]
        public GameSettings GameSettings = new(true, true);
        [SettingName("Universal Projectile Tweaks"), Tooltip("Tweaks that are applied to all projectiles.")]
        public MiscTweaks MiscTweaks = new(true, true, true);
        [SettingName("Arrow Tweaks")]
        public ProjectileTweaks ArrowTweaks = new(true, 5000.0f, 0.34f, 0.44f, Silent, new() { "Arrow" });
        [SettingName("Bolt Tweaks")]
        public ProjectileTweaks BoltTweaks = new(true, 5800.0f, 0.34f, 0.64f, Normal, new() { "Bolt" });
        [SettingName("Throwable Tweaks"), Tooltip("This includes Reikling Spears, and throwing weapons from some other mods.")]
        public ProjectileTweaks ThrowableTweaks = new(true, 2800.0f, 0.13f, 1.1f, Silent, new() { "Riekling", "SSM", "Throw" });
        [SettingName("Blacklist"), Tooltip("Any projectiles specified here will not be modified.")]
        public MatchableRecord blacklist = new(new() { "MQ101ArrowSteelProjectile" }, new());
        [SettingName("Verbose Log"), JsonDiskName("verbose-log"), Tooltip("Writes additional information to the log, useful for debugging.")]
        public bool _use_verbose_log = true;
    }
}