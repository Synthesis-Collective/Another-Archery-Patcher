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
        public FlagTweak(Flag.Type flag, Flag.State state)
        {
            Flag = flag;
            State = state;
        }
        
        public Flag.Type Flag;
        public Flag.State State;
        
        public void Deconstruct(out Flag.Type flag, out Flag.State state)
        {
            flag = Flag;
            state = State;
        }
        
        public void Add(Flag.State state) { }
    }
}
