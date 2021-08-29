using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher.ConfigHelpers
{
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
}
