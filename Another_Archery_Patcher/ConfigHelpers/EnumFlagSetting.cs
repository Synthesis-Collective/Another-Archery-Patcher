using Another_Archery_Patcher.Extensions;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;

namespace Another_Archery_Patcher.ConfigHelpers
{
    public enum EnumFlagOperationType : byte
    {
        Enable,
        Disable,
        Overwrite,
        /// <summary>binary operand <b>|</b></summary>
        BitwiseOR,
        /// <summary>binary operand <b>&amp;</b></summary>
        BitwiseAND,
        /// <summary>binary operand <b>^</b></summary>
        BitwiseXOR,
    }
    public class EnumFlagOperation<T> : IGetValueOrAlternative<T> where T : Enum
    {
        #region Constructors
        public EnumFlagOperation(T value) => Flag = value;
        public EnumFlagOperation(EnumFlagOperationType action, T value) : this(value) => Action = action;
        public EnumFlagOperation() : this(EnumFlagOperationType.Enable, default!) { }
        #endregion Constructors

        #region Properties
        public T Flag;
        /// <summary>
        /// Determines the action that will be performed on the enum value.
        /// </summary>
        [Tooltip("Determines how the associated Flag is applied to the existing value in the Grass record. Don't use the Bitwise operations unless you know what you're doing.")]
        public EnumFlagOperationType Action = EnumFlagOperationType.Enable;

        internal int IntValue
        {
            get => Flag.ToInt();
            set => Flag = value.ToEnum<T>();
        }
        #endregion Properties

        public T GetValueOrAlternative(T inputValue, out bool changed)
        {
            var val = Action switch
            {
                EnumFlagOperationType.Disable => inputValue.BitwiseAND(Flag.BitwiseNOT()),
                EnumFlagOperationType.Enable or EnumFlagOperationType.BitwiseOR => inputValue.BitwiseOR(Flag),
                EnumFlagOperationType.BitwiseAND => inputValue.BitwiseAND(Flag),
                EnumFlagOperationType.BitwiseXOR => inputValue.BitwiseXOR(Flag),
                _ => Flag,
            };
            changed = !val.Equals(inputValue);
            return val;
        }
    }
    public class EnumFlagSetting<T> : IGetValueOrAlternative<T> where T : Enum
    {
        [Tooltip("This MUST be checked to apply the Flag Changes to records. When unchecked, the associated Flag Changes property is skipped, and no changes are made to the original value.")]
        public bool EnableProperty = false;
        [Tooltip("Flag changes are applied in sequential order. Actions that start with \"Bitwise\" are for advanced users.")]
        public List<EnumFlagOperation<T>> FlagChanges = new();

        public T GetValueOrAlternative(T inputValue, out bool changed)
        {
            var val = inputValue;

            foreach (var action in FlagChanges)
            {
                val = action.GetValueOrAlternative(val, out _);
            }

            changed = !val.Equals(inputValue);
            return val;
        }
    }
}
