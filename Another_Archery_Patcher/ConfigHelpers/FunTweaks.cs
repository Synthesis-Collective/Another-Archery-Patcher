using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher.ConfigHelpers
{


    public class FunTweaks {
        public FunTweaks(bool enabled, bool? explode = null, bool? pinLimbs = null)
        {
            Enabled = enabled;
            FlagAllExplosive = explode ?? false;
            FlagAllPinLimbs = pinLimbs ?? false;
        }

        [MaintainOrder]
        public bool Enabled;
        [SettingName("Hitscan"), Tooltip("Makes all projectiles use hitscan.")]
        public bool FlagAllHitscan;
        [SettingName("Explosive"), Tooltip("Makes all projectiles explosive.")]
        public bool FlagAllExplosive;
        [SettingName("Pin Limbs"), Tooltip("Makes all projectiles pin limbs.")]
        public bool FlagAllPinLimbs;

        public Projectile ApplyTweaks(Projectile proj, out uint countModified)
        {
            countModified = 0u;
            if (!Enabled) return proj;
            List<Projectile.Flag> flagList = new();
            if (FlagAllHitscan)
                flagList.Add(Projectile.Flag.Hitscan);
            if (FlagAllExplosive)
                flagList.Add(Projectile.Flag.Explosion);
            if (FlagAllPinLimbs)
                flagList.Add(Projectile.Flag.PinsLimbs);
            FlagEditor.AddFlags(proj, flagList, out countModified);
            return proj;
        }
    }
}
