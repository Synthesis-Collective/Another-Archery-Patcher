using Another_Archery_Patcher.ConfigHelpers.RecordEditor;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Noggog;

namespace Another_Archery_Patcher.ConfigHelpers
{
    /// <summary>
    /// Contains a <see cref="ProjectileFlag.Type"/> and a <see cref="ProjectileFlag.State"/>.
    /// </summary>
    /// <remarks>
    /// This is used instead of a Dictionary because I don't like how Dictionaries look in the settings menu.
    /// </remarks>
    [ObjectNameMember(nameof(Flag))]
    public class FlagTweak
    {
        public FlagTweak(ProjectileFlag.Flag flag, ProjectileFlag.State state)
        {
            Flag = flag;
            State = state;
        }

        public ProjectileFlag.Flag Flag;
        public ProjectileFlag.State State;

        /// <summary>
        /// Allows the <see cref="FlagTweak"/> type to be deconstructed into a tuple.
        /// </summary>
        /// <param name="flag">Item1</param>
        /// <param name="state">Item2</param>
        public void Deconstruct(out ProjectileFlag.Flag flag, out ProjectileFlag.State state)
        {
            flag = Flag;
            state = State;
        }

        public Projectile ApplyTo(Projectile proj)
        {
            proj.Flags.SetFlag((Projectile.Flag)Flag, State == ProjectileFlag.State.Add);
            return proj;
        }

        public Projectile ApplyTo(Projectile proj, out bool changed)
        {
            changed = proj.Flags != proj.Flags.SetFlag((Projectile.Flag)Flag, State == ProjectileFlag.State.Add);
            return proj;
        }
    }
}
