using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;

namespace Another_Archery_Patcher.ConfigHelpers
{
    public class FlagEditor {
        // Add a single flag to a projectile
        public static Projectile AddFlag(Projectile proj, Projectile.Flag flag, out uint modified)
        {
            modified = 0u;
            if ((proj.Flags & flag) == 0) {
                proj.Flags |= flag;
                ++modified;
            }
            return proj;
        }
        // Remove a single flag from a projectile
        public static Projectile RemoveFlag(Projectile proj, Projectile.Flag flag, out uint modified)
        {
            modified = 0u;
            if ((proj.Flags & flag) != 0) {
                proj.Flags &= ~flag;
                ++modified;
            }
            return proj;
        }
        // Add multiple flags to a projectile
        public static Projectile AddFlags(Projectile proj, List<Projectile.Flag> flags, out uint modified)
        {
            modified = 0u;
            foreach (var flag in flags) {
                AddFlag(proj, flag, out var mod);
                modified += mod;
            }
            return proj;
        }
        // Remove multiple flags from a projectile.
        public static Projectile RemoveFlags(Projectile proj, List<Projectile.Flag> flags, out uint modified)
        {
            modified = 0u;
            foreach (var flag in flags) {
                RemoveFlag(proj, flag, out var mod);
                modified += mod;
            }
            return proj;
        }
        // Add a single flag to a projectile
        public static Projectile AddFlag(Projectile proj, Projectile.Flag flag)
        {
            return AddFlag(proj, flag, out _);
        }
        // Remove a single flag from a projectile
        public static Projectile RemoveFlag(Projectile proj, Projectile.Flag flag)
        {
            return RemoveFlag(proj, flag, out _);
        }
        // Add multiple flags to a projectile
        public static Projectile AddFlags(Projectile proj, List<Projectile.Flag> flags)
        {
            return AddFlags(proj, flags, out _);
        }
        // Remove multiple flags from a projectile.
        public static Projectile RemoveFlags(Projectile proj, List<Projectile.Flag> flags)
        {
            return RemoveFlags(proj, flags, out _);
        }
    }
}
