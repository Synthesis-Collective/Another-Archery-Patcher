using System.Collections.Generic;
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
        [SettingName("Projectile Categories"), Tooltip("Various categories of projectiles to patch.")]
        public List<ProjectileTweaks> ProjectileTweaks = new()
        {
            new ProjectileTweaks(5000.0f, 0.34f, 0.44f, Silent, new List<MatchableElement> { new("Arrow") }), ///< @brief Values that are applied to Arrows.
            new ProjectileTweaks(5800.0f, 0.34f, 0.64f, Normal, new List<MatchableElement> { new("Bolt") }), ///< @brief Values that are applied to Bolts.
            new ProjectileTweaks(2800.0f, 0.13f, 1.1f, Silent, new List<MatchableElement> { new("Riekling", false), new("SSM", false), new("Throw", false) }) ///< @brief Values that are applied to throwable weapons, like spears. (In vanilla, this is only applied to the Riekling Spear.)
        };
        [SettingName("Blacklist"), Tooltip("Any projectiles specified here will not be modified.")]
        public MatchableBlacklist Blacklist = new(true, new List<MatchableElement> { new("MQ101ArrowSteelProjectile") }, new List<IFormLinkGetter<IProjectileGetter>>()); ///< @brief Used to ignore certain projectiles used in quests / other projectiles that are sensitive to modification.
        [SettingName("Verbose Log"), JsonDiskName("verbose-log"), Tooltip("Writes additional information to the log, useful for debugging.")]
        public bool UseVerboseLog = true; ///< @brief Toggles verbose console logging.
    }
}