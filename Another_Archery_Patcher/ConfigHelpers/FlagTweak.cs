using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher.ConfigHelpers
{
    /**
     * @class FlagTweak
     * @brief Contains a projectile flag, and a FlagState. This is used instead of a Dictionary because I don't like how Dictionaries look in the settings menu.
     */
    [ObjectNameMember(nameof(Flag))]
    public class FlagTweak
    {
        public FlagTweak(RecordEdit.Editor.Flag.Type flag, RecordEdit.Editor.Flag.State state)
        {
            Flag = flag;
            State = state;
        }

        public RecordEdit.Editor.Flag.Type Flag;
        public RecordEdit.Editor.Flag.State State;

        public void Deconstruct(out RecordEdit.Editor.Flag.Type flag, out RecordEdit.Editor.Flag.State state)
        {
            flag = Flag;
            state = State;
        }
    }
}
