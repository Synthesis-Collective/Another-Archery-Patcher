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
        public GameSettings(bool disableAutoAim, bool disableNpcDodge, int maxAttachedArrows, float fullDrawArrowSpeedMult, int arrowRecoveryChance, float collisionMaxDistFromPlayer)
        {
            DisableAutoaim = disableAutoAim;
            DisableNpcDodge = disableNpcDodge;
            MaxAttachedArrows = maxAttachedArrows > 0 ? maxAttachedArrows : 0;
            FullDrawArrowSpeedMult = fullDrawArrowSpeedMult;
            ArrowRecoveryChance = arrowRecoveryChance > 100 ? 100 : arrowRecoveryChance > 0 ? arrowRecoveryChance : 0;
            CollisionMaxDistFromPlayer = collisionMaxDistFromPlayer;
        }

        [MaintainOrder]
        [SettingName("Disable Auto-Aim")]
        [Tooltip("Removes the terrible vanilla auto-aim from 1st and 3rd person.")]
        public bool DisableAutoaim; ///< @brief Toggles disabling auto-aim.
        [SettingName("Fix Ninja-Dodge")]
        [Tooltip("Prevents NPCs from dodging your arrows at range.")]
        public bool DisableNpcDodge; ///< @brief Toggles disabling NPC dodge to fix npc ninja dodge.
        [SettingName("Max Attached Arrows")]
        [Tooltip("Max number of projectiles stuck into an actor before some start disappearing. (Min: 0, Default: 3)")]
        public int MaxAttachedArrows;
        [SettingName("Fully Drawn Speed Mult")]
        [Tooltip("This is the multiplier applied to arrows shot from a fully drawn bow. (Default: 1.0)")]
        public float FullDrawArrowSpeedMult;
        [SettingName("Recovery Chance")]
        [Tooltip("Chance that a projectile will be in an actors inventory after they are hit by it. (Min: 0, Default: 33, Max: 100)")]
        public int ArrowRecoveryChance;
        [SettingName("fVisibleNavmeshMoveDist")]
        [Tooltip("Maximum distance from the player that projectile collision is calculated by the game engine. (Default: 12288)")]
        public float CollisionMaxDistFromPlayer;


        public void AddGameSettingsToPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (DisableAutoaim)
            {
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fAutoAimMaxDegrees", Data = 0.0f });          // Add new game setting to patch: "fAutoAimMaxDegrees"
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fAutoAimMaxDistance", Data = 0.0f });         // Add new game setting to patch: "fAutoAimMaxDistance"
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fAutoAimScreenPercentage", Data = 0.0f });    // Add new game setting to patch: "fAutoAimScreenPercentage"
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fAutoAimMaxDegrees3rdPerson", Data = 0.0f }); // Add new game setting to patch: "fAutoAimMaxDegrees3rdPerson"
            }
            if (DisableNpcDodge)
                state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fCombatDodgeChanceMax", Data = 0.0f });       // Add new game setting to patch: "fCombatDodgeChanceMax"
            // max attached arrows
            state.PatchMod.GameSettings.Add(new GameSettingInt(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "iMaxAttachedArrows", Data = MaxAttachedArrows });
            // fully drawn arrow speed mult
            state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fArrowSpeedMult", Data = FullDrawArrowSpeedMult });
            // arrow recovery chance
            state.PatchMod.GameSettings.Add(new GameSettingInt(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "iArrowInventoryChance", Data = ArrowRecoveryChance });
            // fVisibleNavmeshMoveDist
            state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = "fVisibleNavmeshMoveDist", Data = CollisionMaxDistFromPlayer });
        }
    }
}
