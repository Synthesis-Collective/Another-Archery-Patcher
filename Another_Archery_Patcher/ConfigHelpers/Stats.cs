using System.Collections.Generic;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher.ConfigHelpers
{
    public class Stats : Matchable
    {
        public Stats(string identifier, int priority, float speed, float gravity, float impactforce, SoundLevel soundlevel, List<string>? matchlist = null)
        {
            Identifier = identifier;
            Priority = priority;
            if (matchlist != null)
                MatchList = matchlist;
            Speed = speed;
            Gravity = gravity;
            ImpactForce = impactforce;
            SoundLevel = soundlevel;
        }

        [MaintainOrder]
        public float Speed;
        public float Gravity;
        [SettingName("Impact Force"), Tooltip("How much force is imparted into objects struck by the projectile.")]
        public float ImpactForce;
        [SettingName("Sound Level"), Tooltip("How much detectable noise a projectile makes while in-flight.")]
        public SoundLevel SoundLevel;

        // private function to perform repetitive getter operations
        private static float GetFloat(float setting, float current, out bool isModified)
        {
            isModified = !setting.Equals(current) && setting >= 0F;
            return isModified ? setting : current;
        }

        public float GetSpeed(float current, out bool isModified)
        {
            return GetFloat(Speed, current, out isModified);
        }

        public float GetGravity(float current, out bool isModified)
        {
            return GetFloat(Gravity, current, out isModified);
        }

        public float GetImpactForce(float current, out bool isModified)
        {
            return GetFloat(ImpactForce, current, out isModified);
        }

        public uint GetSoundLevel(uint current, out bool isModified)
        {
            isModified = (uint)SoundLevel != current;
            return isModified ? (uint)SoundLevel : current;
        }
    }
}
