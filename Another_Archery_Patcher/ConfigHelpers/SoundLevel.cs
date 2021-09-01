using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher.ConfigHelpers
{
    public enum SoundLevel : uint ///< @brief this is needed because reflective settings don't work with Mutagen.Bethesda.Skyrim.SoundLevel for some reason
    {
        [MaintainOrder]
        Silent = 2,
        Normal = 1,
        Loud = 0,
        VeryLoud = 3,
    }
}
