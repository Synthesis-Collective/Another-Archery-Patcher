using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher.ConfigHelpers
{
    /**
     * @class GameSettings
     * @brief Contains all settings related to modifying GMST ( Game Setting ) records.
     */
    public class GameSettings
    {
        public GameSettings(bool disableAutoAim, bool disableNpcDodge, int? maxAttachedArrows, float? fullDrawArrowSpeedMult, int? arrowRecoveryChance, float? maximumProcessingRange)
        {
            Disable_AutoAim = new(new[] { "fAutoAimMaxDegrees", "fAutoAimMaxDistance", "fAutoAimScreenPercentage", "fAutoAimMaxDegrees3rdPerson" }, disableAutoAim ? 0.0f : null);
            Disable_NPCdodge = new("fCombatDodgeChanceMax", disableNpcDodge ? 0.0f : null);
            MaxAttachedArrows = new("iMaxAttachedArrows", (maxAttachedArrows != null && maxAttachedArrows > 0) ? maxAttachedArrows : null);
            FullyDrawnSpeedMult = new("fArrowSpeedMult", fullDrawArrowSpeedMult);
            ArrowRecoveryChance = new("iArrowInventoryChance", arrowRecoveryChance);
            MaximumProcessingRange = new("fVisibleNavmeshMoveDist", maximumProcessingRange);
        }

        [MaintainOrder]
        [SettingName("Disable Auto-Aim"), Tooltip("Removes the terrible vanilla auto-aim from 1st and 3rd person.")]
        public GMST<float> Disable_AutoAim;
        [SettingName("Fix Ninja-Dodge"), Tooltip("Prevents NPCs from dodging your arrows at range.")]
        public GMST<float> Disable_NPCdodge;
        [SettingName("Max Attached Arrows"), Tooltip("Max number of projectiles stuck into an actor before some start disappearing. (Min 0, Default: 3)")]
        public GMST<int> MaxAttachedArrows;
        [SettingName("Fully Drawn Speed Mult"), Tooltip("This is the multiplier applied to arrows shot from a fully drawn bow. (Default: 1)")]
        public GMST<float> FullyDrawnSpeedMult;
        [SettingName("Recovery Chance"), Tooltip("Chance that a projectile will be recoverable after being shot into an actor. (Min 0, Default: 33, Max 100)")]
        public GMST<int> ArrowRecoveryChance;
        [SettingName("fVisibleNavmeshMoveDist"), Tooltip("Maximum distance a projectile can travel before the engine stops processing collisions. (Default: 12288)")]
        public GMST<float> MaximumProcessingRange;

        public void AddGameSettingsToPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            Disable_AutoAim.AddAllSettings(state);
            Disable_NPCdodge.AddAllSettings(state);
            MaxAttachedArrows.AddAllSettings(state);
            FullyDrawnSpeedMult.AddAllSettings(state);
            ArrowRecoveryChance.AddAllSettings(state);
            MaximumProcessingRange.AddAllSettings(state);
        }
    }
}
