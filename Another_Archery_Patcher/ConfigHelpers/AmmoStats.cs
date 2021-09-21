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
        [Tooltip("When Damage == 0 and this is checked, no changes are made.")]
        public bool DamageIsModifier;
        public float Damage;
        [SettingName("Add Value to Current")]
        [Tooltip("When Value == 0 and this is checked, no changes are made.")]
        public bool ValueIsModifier;
        public uint Value;
        [SettingName("Add Weight to Current")]
        [Tooltip("When Weight == 0 and this is checked, no changes are made.")]
        public bool WeightIsModifier;
        public float Weight;

        public float GetDamage(float current, out bool changed)
        {
            if (DamageIsModifier)
            {
                changed = !Damage.EqualsWithin(0);
                return changed ? current + Damage : current;
            }
            else
            {
                changed = current.EqualsWithin(Damage);
                return changed ? Damage : current;
            }
        }

        public uint GetValue(uint current, out bool changed)
        {
            if (ValueIsModifier)
            {
                changed = !Value.Equals(0);
                return changed ? Value : current;
            }
            else
            {
                changed = current.Equals(Value);
                return changed ? Value : current;
            }
        }

        public float GetWeight(float current, out bool changed)
        {
            if (WeightIsModifier)
            {
                changed = !Weight.EqualsWithin(0);
                return changed ? Weight + current : current;
            }
            else
            {
                changed = current.EqualsWithin(Weight);
                return changed ? Weight : current;
            }
        }

        public bool ShouldSkip()
        {
            return Damage.EqualsWithin(0) && DamageIsModifier && Value.Equals(0) && ValueIsModifier && Weight.EqualsWithin(0) && WeightIsModifier;
        }

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
