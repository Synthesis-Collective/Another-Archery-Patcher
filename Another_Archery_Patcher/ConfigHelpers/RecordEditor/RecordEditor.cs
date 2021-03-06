using Mutagen.Bethesda.Skyrim;
using System.Collections.Generic;

namespace Another_Archery_Patcher.ConfigHelpers.RecordEdit
{
    public readonly partial struct Editor
    {
        public readonly struct Flag
        {
            public enum State : uint
            {
                Remove = 0u,
                Add = 1u
            }

            [System.Flags]
            public enum Type // needed for settings to show up. Don't include 0 none value as users shouldn't be able to select it.
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

            // Add a single flag to a projectile
            public static (Projectile, uint) AddFlag(Projectile proj, Projectile.Flag flag)
            {
                var modified = 0u;
                if ((proj.Flags & flag) == 0)
                {
                    proj.Flags |= flag;
                    ++modified;
                }
                return (proj, modified);
            }
            // Remove a single flag from a projectile
            public static (Projectile, uint) RemoveFlag(Projectile proj, Projectile.Flag flag)
            {
                var modified = 0u;
                if ((proj.Flags & flag) != 0)
                {
                    proj.Flags &= ~flag;
                    ++modified;
                }
                return (proj, modified);
            }
            // Add multiple flags to a projectile
            public static (Projectile, uint) AddFlags(Projectile proj, List<Projectile.Flag> flags)
            {
                var modified = 0u;
                foreach (var flag in flags)
                {
                    uint mod;
                    (proj, mod) = AddFlag(proj, flag);
                    modified += mod;
                }
                return (proj, modified);
            }
            // Remove multiple flags from a projectile.
            public static (Projectile, uint) RemoveFlags(Projectile proj, List<Projectile.Flag> flags)
            {
                var modified = 0u;
                foreach (var flag in flags)
                {
                    uint mod;
                    (proj, mod) = RemoveFlag(proj, flag);
                    modified += mod;
                }
                return (proj, modified);
            }
            // Apply a FlagState to a projectile.
            public static (Projectile, uint) ApplyFlag(Projectile proj, Projectile.Flag flag, State state)
            {
                return state switch
                {
                    State.Add => AddFlag(proj, flag),
                    State.Remove => RemoveFlag(proj, flag),
                    _ => (proj, 0)
                };
            }

            public static (Projectile, uint) ApplyFlags(Projectile proj, List<FlagTweak> flags)
            {
                var modified = 0u;
                foreach (var (flag, state) in flags)
                {
                    uint mod;
                    (proj, mod) = ApplyFlag(proj, (Projectile.Flag)flag, state);
                    modified += mod;
                }
                return (proj, modified);
            }
        }
    }
}
