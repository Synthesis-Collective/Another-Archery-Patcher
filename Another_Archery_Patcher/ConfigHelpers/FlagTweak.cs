using Another_Archery_Patcher.ConfigHelpers.RecordEditor;
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
        public FlagTweak(Editor.Flag.Type flag, Editor.Flag.State state)
        {
            Flag = flag;
            State = state;
        }

        public Editor.Flag.Type Flag;
        public Editor.Flag.State State;

        public void Deconstruct(out Editor.Flag.Type flag, out Editor.Flag.State state)
        {
            flag = Flag;
            state = State;
        }
    }
}
