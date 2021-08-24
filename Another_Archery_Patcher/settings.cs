using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher
{
    using static SoundLevel;
    public enum SoundLevel ///< @brief this is needed because reflective settings don't work with Mutagen.Bethesda.Skyrim.SoundLevel for some reason
    {
        [MaintainOrder]
        Silent = 2,
        Normal = 1,
        Loud = 0,
        VeryLoud = 3,
    }
    /**
     * @class GameSettings
     * @brief Contains all settings related to modifying GMST ( Game Setting ) records.
     */
    public class GameSettings
    {
        public GameSettings(bool disableAutoAim, bool disableNpcDodge)
        {
            DisableAutoaim = disableAutoAim;
            DisableNpcDodge = disableNpcDodge;
        }
        [MaintainOrder]
        [SettingName("Disable Auto-Aim"), Tooltip("Removes the terrible vanilla auto-aim from 1st and 3rd person.")]
        public bool DisableAutoaim; ///< @brief Toggles disabling auto-aim.
        [SettingName("Patch NPC Ninja Dodge Bug"), Tooltip("Prevents NPCs from dodging your arrows at range.")]
        public bool DisableNpcDodge; ///< @brief Toggles disabling NPC dodge to fix the infamous "Ninja Dodge" bug.
    }
    /**
     * @class MiscTweaks
     * @brief Contains settings that are applied to all PROJ records, and additional toggles that don't fit into the GameSettings category.
     */
    public class MiscTweaks
    {
        public MiscTweaks(bool disableSupersonicFlag, bool removeBloodcursedGravity, bool patchTraps)
        {
            DisableSupersonic = disableSupersonicFlag;
            DisableGravityBloodcursed = removeBloodcursedGravity;
            PatchTraps = patchTraps;
        }
        [MaintainOrder]
        [SettingName("Remove Supersonic Flag"), Tooltip("Remove the supersonic flag from projectiles of this type. The supersonic flag removes sound from in-flight projectiles.")]
        public bool DisableSupersonic;
        [Ignore] public List<string> BloodcursedId = new() { "DCL1ArrowElvenBloodProjectile", "DLC1AurielsBloodDippedProjectile" }; // Editor ID list of bloodcursed arrows
        [SettingName("No Gravity for Bloodcursed Arrows"), Tooltip("Makes it easier/possible to hit the sun with the Dawnguard DLC's Bloodcursed Elven Arrows")]
        public bool DisableGravityBloodcursed;
        [SettingName("Patch Trap Projectiles"), Tooltip("Modifies most of the projectiles fired by dart traps & dwemer ballista traps to be more interesting. (If anyone wants to customize the values, let me know and I'll add it)")]
        public bool PatchTraps;
    }

    public class MatchableElement
    {
        protected bool Equals(MatchableElement other)
        {
            return Name == other.Name && Required == other.Required;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MatchableElement)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Required);
        }

        public MatchableElement(string name, bool required = false)
        {
            Name = name;
            Required = required;
        }
        [MaintainOrder]
        [Tooltip("This is the string to search for in each Projectile's Editor ID (Or Form ID)")]
        public string Name;
        [Tooltip("When true, a projectile is required to match this entry to be considered part of this category, even if it matches other non-required entries too.")]
        public bool Required;

        public bool IsMatch(string? id)
        {
            if (id == null) return false;
            return Name == id || id.Contains(Name, StringComparison.OrdinalIgnoreCase) || Name.Contains(id, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return Name;
        }

        public static bool operator ==(MatchableElement elem, string comp)
        {
            return elem.Name == comp;
        }
        public static bool operator !=(MatchableElement elem, string comp)
        {
            return elem.Name == comp;
        }
    }
    /**
     * @class Matchable
     * @brief Base class that allows disabling itself, and exposes a list of strings used for matching records against it.
     */
    public class Matchable
    {
        public Matchable(List<MatchableElement>? matchlist)
        {
            Matchlist = matchlist ?? new List<MatchableElement>();
        }

        private bool ContainsRequired(string? id)
        {
            return id != null && Matchlist.All(elem => !elem.Required || elem.Required && elem.IsMatch(id));
        }
        private bool Contains(string? id)
        {
            return id != null && Matchlist.Any(elem => elem.IsMatch(id)) && ContainsRequired(id);
        }
        /**
         * @brief Checks if the given Editor ID or Form ID is a case-insensitive match for any of the strings in the matchlist.
         */
        public bool IsMatch(string? id)
        {
            if (id == null || Matchlist.Count <= 0) return false;
            return Contains(id);
        }
        [MaintainOrder]
        [SettingName("Common Names"), JsonDiskName("matchlist"), Tooltip("(Don't change this unless you know what you're doing!) Used to resolve projectile type, as there is no other way to distinguish between arrows/bolts/other")]
        public List<MatchableElement> Matchlist;
    }
    /**
     * @class MatchableRecord
     * @brief Represents a list of Records that are used for the record blacklisting feature.
     */
    public class MatchableBlacklist : Matchable
    {
        /**
         * @brief Default constructor that takes a list of blacklisted IDs & list of blacklisted Record FormLinks
         */
        public MatchableBlacklist(bool enabled, List<MatchableElement> blacklistedIDs, List<IFormLinkGetter<IProjectileGetter>> blacklistedRecords) : base(blacklistedIDs)
        {
            Enabled = enabled;
            Record = blacklistedRecords;
        }

        [MaintainOrder]
        [Tooltip("Disabling this will disable this blacklist.")]
        public bool Enabled;
        [SettingName("Records")]
        public List<IFormLinkGetter<IProjectileGetter>> Record;
    }
    /**
     * @class ProjectileStats
     * @brief Contains common DATA stats for PROJ records.
     */
    public class ProjectileStats // Contains projectile stats only. Used to maintain ordering with inheritance
    {
        /**
         * @brief Default Constructor.
         */
        public ProjectileStats(float projSpeed, float projGravity, float projImpactForce, SoundLevel projSoundLevel)
        {
            Speed = projSpeed;
            Gravity = projGravity;
            ImpactForce = projImpactForce;
            SoundLevel = projSoundLevel;
        }
        [MaintainOrder]
        [SettingName("Speed"), Tooltip("The speed of this type of projectile. Controls projectile drop.")]
        public float Speed; ///< @brief DATA\Speed value, controls how fast the projectile moves while in-flight.
        [SettingName("Gravity"), Tooltip("The amount of gravity applied to this type of projectile. Controls projectile drop.")]
        public float Gravity; ///< @brief DATA\Gravity value, controls how much gravity is applied to the projectile while in-flight.
        [SettingName("Impact Force"), Tooltip("The amount of force imparted into objects hit by projectiles of this type.")]
        public float ImpactForce; ///< @brief DATA\Impact Force value, controls how much force is imparted into objects the projectile collides with.
        [SettingName("Sound Level"), Tooltip("The amount of detectable noise produced by in-flight projectiles.")]
        public SoundLevel SoundLevel; ///< @brief VNAM\Sound Level value, controls how much detectable noise is generated by the projectile while in-flight.
    }
    /**
     * @class ProjectileTweaks
     * @brief Represents a section of the settings menu that is applied to a subset of projectiles.
     */
    public class ProjectileTweaks : Matchable
    {
        /**
         * @brief Default constructor that takes all parameters required by both the Matchable class & ProjectileStats class.
         */
        public ProjectileTweaks(float projSpeed, float projGravity, float projImpactForce, SoundLevel projSoundLevel, List<MatchableElement>? matchableIds = null) : base(matchableIds)
        {
            Stats = new ProjectileStats(projSpeed, projGravity, projImpactForce, projSoundLevel);
        }
        [MaintainOrder]
        [SettingName("Stats")]
        public ProjectileStats Stats;

        ///< @brief Contains the values to be applied to projectiles in this group.

        public string GetMatchlistAsString()
        {
            return Matchlist.Aggregate("", (current, match) => current + (match.ToString() + ", "));
        }

        public string GetVarsAsString()
        {
            string str = "( ";
            str += "Speed:" + Stats.Speed + ", ";
            str += "Gravity:" + Stats.Gravity + ", ";
            str += "ImpactForce:" + Stats.ImpactForce + ", ";
            str += "SoundLevel:" + Stats.SoundLevel + ", ";
            str += "Matchlist:[ ";
            str += Matchlist.Aggregate(str, (current, match) => current + (match.ToString() + ", "));
            str += " ]";
            return str + " )";
        }
    }
    /**
     * @class TopLevelSettings
     * @brief Contains all settings used by the patcher.
     */
    public class TopLevelSettings
    {
        [MaintainOrder]
        [SettingName("Game Setting Tweaks")]
        public GameSettings GameSettings = new(true, true); ///< @brief Contains toggles for Game Setting changes.
        [SettingName("Universal Projectile Tweaks"), Tooltip("Tweaks that are applied to all projectiles.")]
        public MiscTweaks MiscTweaks = new(true, true, true); ///< @brief Contains toggles for miscellaneous/global tweaks that are applied to all modified records.
        [SettingName("Projectile Tweaks"), Tooltip("Various categories of projectiles to patch.")]
        public List<ProjectileTweaks> ProjectileTweaks = new()
        {
            new ProjectileTweaks(5000.0f, 0.34f, 0.44f, Silent, new List<MatchableElement> { new("Arrow") }), ///< @brief Values that are applied to Arrows.
            new ProjectileTweaks(5800.0f, 0.34f, 0.64f, Normal, new List<MatchableElement> { new("Bolt") }), ///< @brief Values that are applied to Bolts.
            new ProjectileTweaks(2800.0f, 0.13f, 1.1f, Silent, new List<MatchableElement> { new("Riekling"), new("SSM"), new("Throw") }) ///< @brief Values that are applied to throwable weapons, like spears. (In vanilla, this is only applied to the Riekling Spear.)
        };
        [SettingName("Blacklist"), Tooltip("Any projectiles specified here will not be modified.")]
        public MatchableBlacklist Blacklist = new(true, new List<MatchableElement> { new("MQ101ArrowSteelProjectile") }, new List<IFormLinkGetter<IProjectileGetter>>()); ///< @brief Used to ignore certain projectiles used in quests / other projectiles that are sensitive to modification.
        [SettingName("Verbose Log"), JsonDiskName("verbose-log"), Tooltip("Writes additional information to the log, useful for debugging.")]
        public bool UseVerboseLog = true; ///< @brief Toggles verbose console logging.
    }
}