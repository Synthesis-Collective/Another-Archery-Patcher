namespace Another_Archery_Patcher.ConfigHelpers.RecordEditor
{
    public readonly struct ProjectileFlag
    {
        public enum State : uint
        {
            Remove = 0u,
            Add = 1u
        }

        [System.Flags]
        public enum Flag // needed for settings to show up. Don't include 0 none value as users shouldn't be able to select it.
        {
            Hitscan = 1,
            Explosion = 2,
            AltTrigger = 4,
            MuzzleFlash = 8,
            CanBeDisabled = 32, // 0x00000020
            CanBePickedUp = 64, // 0x00000040
            Supersonic = 128, // 0x00000080
            PinsLimbs = 256, // 0x00000100
            PassThroughSmallTransparent = 512, // 0x00000200
            DisableCombatAimCorrection = 1024, // 0x00000400
            Rotation = 32768, // 0x00008000
        }

        public enum Type
        {
            Missile = 1,
            Lobber = 2,
            Beam = 4,
            Flame = 8,
            Cone = 16,
            Barrier = 32,
            Arrow = 64
        }
    }
}
