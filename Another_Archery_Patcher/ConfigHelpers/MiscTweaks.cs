using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher.ConfigHelpers
{
    /**
     * @class MiscTweaks
     * @brief Contains settings that are applied to all PROJ records, and additional toggles that don't fit into the GameSettings category.
     */
    public class MiscTweaks
    {
        public MiscTweaks(bool disableSupersonicFlag, bool removeBloodcursedGravity, bool patchTraps, bool disableCombatAutoAim)
        {
            DisableSupersonic = disableSupersonicFlag;
            DisableGravityBloodcursed = removeBloodcursedGravity;
            PatchTraps = patchTraps;
            FlagAllDisableCombatAutoAim = disableCombatAutoAim;
        }

        [MaintainOrder]
        [SettingName("Remove Supersonic Flag"), Tooltip("Remove the supersonic flag from projectiles of this type. The supersonic flag removes sound from in-flight projectiles.")]
        public bool DisableSupersonic;
        [SettingName("No Gravity for Bloodcursed Arrows"), Tooltip("Makes it easier/possible to hit the sun with the Dawnguard DLC's Bloodcursed Elven Arrows")]
        public bool DisableGravityBloodcursed;
        [SettingName("Patch Trap Projectiles"), Tooltip("Modifies most of the projectiles fired by dart traps & dwemer ballista traps to be more interesting. (If anyone wants to customize the values, let me know and I'll add it)")]
        public bool PatchTraps;
        [SettingName("(Experimental) Flag all as No-Auto-Aim")] 
        public bool FlagAllDisableCombatAutoAim;
    }

}
