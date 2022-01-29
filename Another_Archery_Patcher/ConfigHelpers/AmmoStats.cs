using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Noggog;


namespace Another_Archery_Patcher.ConfigHelpers
{
    public class AmmoStats
    {
        public AmmoStats()
        {
            DamageIsModifier = true;
            Damage = 0.0f;
            ValueIsModifier = true;
            Value = 0u;
            WeightIsModifier = true;
            Weight = 0.0f;
        }
        public AmmoStats(bool damageModifier, float damage, bool valueModifier, uint value, bool weightModifier, float weight)
        {
            DamageIsModifier = damageModifier;
            Damage = damage;
            ValueIsModifier = valueModifier;
            Value = value;
            WeightIsModifier = weightModifier;
            Weight = weight;
        }

        [SettingName("Add Damage to Current")]
        [Tooltip("If unchecked, damage values are overwritten. When Damage == 0 and this is checked, no changes are made.")]
        public bool DamageIsModifier;
        public float Damage;
        [SettingName("Add Value to Current")]
        [Tooltip("If unchecked, gold values are overwritten. When Value == 0 and this is checked, no changes are made.")]
        public bool ValueIsModifier;
        public uint Value;
        [SettingName("Add Weight to Current")]
        [Tooltip("If unchecked, weight values are overwritten. When Weight == 0 and this is checked, no changes are made.")]
        public bool WeightIsModifier;
        public float Weight;

        /// <summary>
        /// Retrieve the modified damage of an AMMO record, according to the user's configuration.
        /// Also checks if the value was modified to avoid ITPOs & count changes.
        /// </summary>
        /// <param name="current">The current damage of an AMMO record.</param>
        /// <param name="changed">An output variable that when true, indicates that the record is not an ITPO and should be added to the patch.</param>
        /// <returns>(float): The modified damage.</returns>
        public float GetDamage(float current, out bool changed)
        {
            float newDamage = DamageIsModifier
                ? (current + Damage)
                : Damage;
            // change when the current value is not equal to the modified value
            changed = !current.EqualsWithin(newDamage);
            return changed ? newDamage : current;
        }

        /// <summary>
        /// Retrieve the modified value of an AMMO record, according to the user's configuration.
        /// Also checks if the value was modified to avoid ITPOs & count changes.
        /// </summary>
        /// <param name="current">The current value of an AMMO record.</param>
        /// <param name="changed">An output variable that when true, indicates that the record is not an ITPO and should be added to the patch.</param>
        /// <returns>(float): The modified value.</returns>
        public uint GetValue(uint current, out bool changed)
        {
            uint newValue = ValueIsModifier
                ? (current + Value)
                : Value;
            // change when the current value is not equal to the modified value
            changed = !current.Equals(newValue);
            return changed ? newValue : current;
        }

        /// <summary>
        /// Retrieve the modified weight of an AMMO record, according to the user's configuration.
        /// Also checks if the value was modified to avoid ITPOs & count changes.
        /// </summary>
        /// <param name="current">The current weight</param>
        /// <param name="changed">An output variable that when true, indicates that the record is not an ITPO and should be added to the patch.</param>
        /// <returns>(float): The modified weight.</returns>
        public float GetWeight(float current, out bool changed)
        {
            float newWeight = WeightIsModifier
                ? (current + Weight)
                : Weight;
            // change when the current value is not equal to the modified value
            changed = !current.EqualsWithin(newWeight);
            return changed ? newWeight : current;
        }

        /// <summary>
        /// Check if this entry should be skipped by checking if there are any values that aren't modifiers set to 0.
        /// </summary>
        /// <returns>(bool): True when there are valid values to apply to an AMMO record.</returns>
        public bool ShouldSkip()
        {
            return Damage.EqualsWithin(0) && DamageIsModifier && Value.Equals(0) && ValueIsModifier && Weight.EqualsWithin(0) && WeightIsModifier;
        }

        /// <summary>
        /// Accepts an Ammunition object, modifies it, and returns the modified copy as well as the number of changes.
        /// </summary>
        /// <param name="ammo">An Ammunition object, retrieved by performing a deep copy of a record.</param>
        /// <returns>((Ammunition): The modified ammunition object,(int): The number of changes made to the object.)</returns>
        public (Ammunition, int) ApplySettingsTo(Ammunition ammo)
        {
            var count = 0;
            ammo.Damage = GetDamage(ammo.Damage, out var changed);
            count += changed ? 1 : 0;
            ammo.Value = GetValue(ammo.Value, out changed);
            count += changed ? 1 : 0;
            ammo.Weight = GetWeight(ammo.Weight, out changed);
            count += changed ? 1 : 0;
            return (ammo, count);
        }
    }
}
