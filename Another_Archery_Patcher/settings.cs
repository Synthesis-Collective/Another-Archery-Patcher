using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public GameSettings(bool disableAutoAim, bool disableNpcDodge)
        {
            DisableAutoaim = disableAutoAim;
            DisableNpcDodge = disableNpcDodge;
        }
        [MaintainOrder]
        [SettingName("Disable Auto-Aim"), Tooltip("Removes the terrible vanilla auto-aim from 1st and 3rd person.")]
        public bool DisableAutoaim;
        [SettingName("Patch NPC Ninja Dodge Bug"), Tooltip("Prevents NPCs from dodging your arrows at range.")]
        public bool DisableNpcDodge;
    }
    public class MiscTweaks // misc projectile tweaks
    {
        public MiscTweaks(bool disableSupersonicFlag, bool removeBloodcursedGravity, bool patchTraps)
        {
            DisableSupersonic = disableSupersonicFlag;
            DisableGravityBloodcursed = removeBloodcursedGravity;
            PatchTraps = patchTraps;
        }
        [MaintainOrder]
        [Tooltip("Remove the supersonic flag from projectiles of this type. The supersonic flag removes sound from in-flight projectiles.")]
        public bool DisableSupersonic;
        [Ignore] public List<string> BloodcursedId = new() { "DCL1ArrowElvenBloodProjectile", "DLC1AurielsBloodDippedProjectile" }; // Editor ID list of bloodcursed arrows
        [SettingName("No Gravity for Bloodcursed Arrows"), Tooltip("Makes it easier/possible to hit the sun with the Dawnguard DLC's Bloodcursed Elven Arrows")]
        public bool DisableGravityBloodcursed;
        [SettingName("Patch Trap Projectiles"), Tooltip("Modifies most of the projectiles fired by dart traps & dwemer ballista traps to be more interesting. (If anyone wants to customize the values, let me know and I'll add it)")]
        public bool PatchTraps;
    }
    public class Matchable // Base class used to indicate that an object can be matched against editor IDs
    {
        public Matchable(List<string>? matchlist, bool enabled = true)
        {
            Enabled = enabled;
            Matchlist = matchlist ?? (new List<string>());
        }
        public bool IsMatch(string? id, bool allowPartialMatch = true)
        {
            if (id == null || !Enabled || Matchlist.Count <= 0) return false;
            return !allowPartialMatch ? Matchlist.Contains(id) : Matchlist.Any(comp => id.Contains(comp, StringComparison.OrdinalIgnoreCase) || comp == id);
        }
        [MaintainOrder]
        [SettingName("Enable"), JsonDiskName("enabled")]
        public bool Enabled;
        [SettingName("Common Names"), JsonDiskName("matchlist"), Tooltip("(Don't change this unless you know what you're doing!) Used to resolve projectile type, as there is no other way to distinguish between arrows/bolts/other")]
        public List<string> Matchlist;
    }
    public class MatchableRecord : Matchable
    {
        public MatchableRecord(List<string> blacklistedIDs, List<IFormLinkGetter<IProjectileGetter>> blacklistedRecords) : base(blacklistedIDs)
        {
            Record = blacklistedRecords;
        }
        [MaintainOrder]
        [SettingName("Records")]
        public List<IFormLinkGetter<IProjectileGetter>> Record;
    }
    public class ProjectileStats // Contains projectile stats only. Used to maintain ordering with inheritance
    {
        public ProjectileStats(float projSpeed, float projGravity, float projImpactForce, SoundLevel projSoundLevel)
        {
            Speed = projSpeed;
            Gravity = projGravity;
            ImpactForce = projImpactForce;
            SoundLevel = projSoundLevel;
        }
        [MaintainOrder]
        [SettingName("Speed"), Tooltip("The speed of this type of projectile. Controls projectile drop.")]
        public float Speed;
        [SettingName("Gravity"), Tooltip("The amount of gravity applied to this type of projectile. Controls projectile drop.")]
        public float Gravity;
        [SettingName("Impact Force"), Tooltip("The amount of force imparted into objects hit by projectiles of this type.")]
        public float ImpactForce;
        [SettingName("Sound Level"), Tooltip("The amount of detectable noise produced by in-flight projectiles.")]
        public SoundLevel SoundLevel;
    }

    public class ProjectileTweaks : Matchable // Matchable projectile stats wrapper
    {
        public ProjectileTweaks(bool enable, float projSpeed, float projGravity, float projImpactForce, SoundLevel projSoundLevel, List<string>? matchableIds = null) : base(matchableIds, enable)
        {
            Stats = new ProjectileStats(projSpeed, projGravity, projImpactForce, projSoundLevel);
        }
        [MaintainOrder]
        [SettingName("Stats")]
        public ProjectileStats Stats;
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
        public MatchableRecord Blacklist = new(new() { "MQ101ArrowSteelProjectile" }, new());
        [SettingName("Verbose Log"), JsonDiskName("verbose-log"), Tooltip("Writes additional information to the log, useful for debugging.")]
        public bool UseVerboseLog = true;
    }
}