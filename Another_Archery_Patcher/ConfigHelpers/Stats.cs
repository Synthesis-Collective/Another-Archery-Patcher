using Another_Archery_Patcher.ConfigHelpers.RecordEditor;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Another_Archery_Patcher.ConfigHelpers
{
    [ObjectNameMember(nameof(Identifier))]
    public class Stats
    {
        public Stats(string identifier, int priority, float speed, float gravity, float impactforce, SoundLevel soundlevel, List<string>? matchlist = null, List<EnumFlagOperation<Editor.Flag.Type>>? flags = null)
        {
            Identifier = identifier;
            Priority = priority;
            Speed = speed;
            Gravity = gravity;
            ImpactForce = impactforce;
            SoundLevel = soundlevel;
            Flags = new() { FlagChanges = flags ?? new() };
            MatchList = matchlist ?? new List<string>();
        }

        [MaintainOrder]
        [Tooltip("The name used to identify this category. Not used by the patcher.")]
        public string Identifier;
        [Tooltip("The highest-priority applicable category is always used in the event of conflicting categories.")]
        public int Priority;
        public float Speed;
        public float Gravity;
        [SettingName("Impact Force")]
        [Tooltip("How much force is imparted into objects struck by the projectile.")]
        public float ImpactForce;
        [SettingName("Sound Level")]
        [Tooltip("How much detectable noise a projectile makes while in-flight.")]
        public SoundLevel SoundLevel;
        [SettingName("Flags")]
        [Tooltip("Add or remove flags from this category only.")]
        public EnumFlagSetting<Editor.Flag.Type> Flags;
        [Tooltip("List of words that must appear in a projectile's EditorID to be considered applicable. Leave empty to match all.")]
        public List<string> MatchList;

        private static T ResolveValue<T>(T settingVal, T currentVal, out bool modified)
        {
            modified = !settingVal!.Equals(currentVal); // check if values are equal
            return modified ? settingVal : currentVal; // return preferred value
        }

        public float GetSpeed(float current, out bool modified)
        {
            return ResolveValue(Speed, current, out modified);
        }

        public float GetGravity(float current, out bool modified)
        {
            return ResolveValue(Gravity, current, out modified);
        }

        public float GetImpactForce(float current, out bool modified)
        {
            return ResolveValue(ImpactForce, current, out modified);
        }

        public uint GetSoundLevel(uint current, out bool modified)
        {
            return ResolveValue((uint)SoundLevel, current, out modified);
        }

        /**
         * @brief Checks if a given id contains any string in the MatchList, or if the list is empty.
         * @param id        - The Editor ID of the record to check.
         * @returns bool
         *\n        true    - The matchlist is empty, or the given ID contains at least one match.
         *\n        false   - The matchlist is not empty and the given ID does not contain any of them.
         */
        private bool HasMatch(string id)
        {
            return MatchList.Count == 0 || MatchList.Any(match => id.Contains(match, StringComparison.OrdinalIgnoreCase));
        }

        /**
         * @brief Retrieves the priority of this stats instance if it applies, otherwise returns a given value.
         * @param id            - The Editor ID of the record to check.
         * @param defPriority   - Default priority value to return if HasMatch returns false.
         * @returns int         - The priority level of this Stats instance if HasMatch returned true, else the value of defPriority.
         */
        public int GetPriority(string id, int defPriority = -1)
        {
            return HasMatch(id) ? Priority : defPriority;
        }
    }
}
