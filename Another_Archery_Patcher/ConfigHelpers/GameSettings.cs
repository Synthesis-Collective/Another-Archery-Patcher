using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;

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
        [Tooltip("Disables the built-in auto-aim from 1st & 3rd person.")]
        public bool DisableAutoaim; ///< @brief Toggles disabling auto-aim.
        [SettingName("Fix Ninja-Dodge")]
        [Tooltip("Prevents a bug where NPCs teleport out of the way when they are about to be shot at a distance, even when they haven't detected you.")]
        public bool DisableNpcDodge; ///< @brief Toggles disabling NPC dodge to fix npc ninja dodge.
        [SettingName("Max Attached Arrows")]
        [Tooltip("Max number of projectiles stuck into an actor before some start disappearing. (Min: 0, Default: 3)")]
        public int MaxAttachedArrows;
        [SettingName("Fully Drawn Speed Mult")]
        [Tooltip("This is the speed multiplier applied to arrows shot from a fully drawn bow. (Default: 1.0)")]
        public float FullDrawArrowSpeedMult;
        [SettingName("Recovery Chance")]
        [Tooltip("Chance that a projectile will be in an actors inventory after they are hit by it. (Min: 0, Default: 33, Max: 100)")]
        public int ArrowRecoveryChance;
        [SettingName("fVisibleNavmeshMoveDist")]
        [Tooltip("Maximum distance from the player that projectile collision is calculated by the game engine. (Default: 12288)")]
        public float CollisionMaxDistFromPlayer;


        private bool AddToPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, string editorID, object data)
        {
            if (state.LinkCache.TryResolveIdentifier<IGameSettingGetter>(editorID, out FormKey formKey) && state.LinkCache.TryResolve<IGameSettingGetter>(formKey, out IGameSettingGetter? gameSettingGetter))
            { // RECORD ALREADY EXISTS:
                var gameSettingCopy = gameSettingGetter.DeepCopy();

                if (gameSettingCopy is GameSettingFloat floatGameSetting)
                {
                    var val = Convert.ToSingle(data);
                    if (val.Equals(floatGameSetting.Data))
                        return false;

                    floatGameSetting.Data = val;
                }
                else if (gameSettingCopy is GameSettingInt intGameSetting)
                {
                    var val = Convert.ToInt32(data);
                    if (val.Equals(intGameSetting.Data))
                        return false;

                    intGameSetting.Data = val;
                }
                else if (gameSettingCopy is GameSettingString strGameSetting)
                {
                    var val = Convert.ToString(data);
                    if (val is null || val.Equals(strGameSetting.Data))
                        return false;

                    strGameSetting.Data = val;
                }
                else if (gameSettingCopy is GameSettingBool boolGameSetting)
                {
                    var val = Convert.ToBoolean(data);
                    if (val.Equals(boolGameSetting.Data))
                        return false;

                    boolGameSetting.Data = val;
                }
                else throw new InvalidOperationException($"The type '{data.GetType().FullName}' is invalid for parameter '{nameof(data)}' in function '{nameof(AddToPatch)}'; expected 'float', 'int', 'string', or 'bool'!");

                state.PatchMod.GameSettings.Set(gameSettingCopy);
                return true;
            }
            else
            { // RECORD DOES NOT EXIST:
                GameSetting? gameSetting = null;

                if (data is float floatVal)
                {
                    gameSetting = new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = editorID, Data = floatVal };
                }
                else if (data is int intVal)
                {
                    gameSetting = new GameSettingInt(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = editorID, Data = intVal };
                }
                else if (data is string || data is null)
                {
                    gameSetting = new GameSettingString(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = editorID, Data = (string?)data };
                }
                else if (data is bool boolVal)
                {
                    gameSetting = new GameSettingBool(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease) { EditorID = editorID, Data = boolVal };
                }
                else throw new InvalidOperationException($"The type '{data.GetType().FullName}' is invalid for parameter '{nameof(data)}' in function '{nameof(AddToPatch)}'; expected 'float', 'int', 'string', or 'bool'!");

                state.PatchMod.GameSettings.Set(gameSetting);
                return true;
            }
        }
        public void AddGameSettingsToPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (DisableAutoaim)
            {
                AddToPatch(state, "fAutoAimMaxDegrees", 0.0f);
                AddToPatch(state, "fAutoAimMaxDistance", 0.0f);
                AddToPatch(state, "fAutoAimScreenPercentage", 0.0f);
                AddToPatch(state, "fAutoAimMaxDegrees3rdPerson", 0.0f);
            }
            if (DisableNpcDodge)
            {
                AddToPatch(state, "fCombatDodgeChanceMax", 0.0f);
            }
            // max attached arrows
            AddToPatch(state, "iMaxAttachedArrows", MaxAttachedArrows);
            // fully drawn arrow speed mult
            AddToPatch(state, "fArrowSpeedMult", FullDrawArrowSpeedMult);
            // arrow recovery chance
            AddToPatch(state, "iArrowInventoryChance", ArrowRecoveryChance);
            // fVisibleNavmeshMoveDist
            AddToPatch(state, "fVisibleNavmeshMoveDist", CollisionMaxDistFromPlayer);
        }
    }
}
