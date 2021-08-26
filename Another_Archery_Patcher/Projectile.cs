using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher
{
    /**
     * @class ProjectileStats
     * @brief Contains common DATA stats for PROJ records.
     */
    public class ProjectileStats // Contains projectile stats only. Used to maintain ordering with inheritance
    {
        public ProjectileStats(float projSpeed, float projGravity, float projImpactForce, SoundLevel projSoundLevel)
        {
            Speed = projSpeed;
            Gravity = projGravity;
            ImpactForce = projImpactForce;
            SoundLevel = projSoundLevel;
        }
        [MaintainOrder]
        [SettingName("Speed"), Tooltip("The speed of this type of projectile. Controls projectile drop.")]
        public float Speed; ///< @brief DATA\Speed value, controls how fast the projectile moves while in-flight.
        [SettingName("Gravity"), Tooltip("The amount of gravity applied to this type of projectile. Controls projectile drop.")]
        public float Gravity; ///< @brief DATA\Gravity value, controls how much gravity is applied to the projectile while in-flight.
        [SettingName("Impact Force"), Tooltip("The amount of force imparted into objects hit by projectiles of this type.")]
        public float ImpactForce; ///< @brief DATA\Impact Force value, controls how much force is imparted into objects the projectile collides with.
        [SettingName("Sound Level"), Tooltip("The amount of detectable noise produced by in-flight projectiles.")]
        public SoundLevel SoundLevel; ///< @brief VNAM\Sound Level value, controls how much detectable noise is generated by the projectile while in-flight.
    }
    /**
     * @class ProjectileTweaks
     * @brief Represents a section of the settings menu that is applied to a subset of projectiles.
     */
    public class ProjectileTweaks : Matchable
    {
        public ProjectileTweaks(float projSpeed, float projGravity, float projImpactForce, SoundLevel projSoundLevel, List<MatchableElement>? matchableIds = null) : base(matchableIds)
        {
            Stats = new ProjectileStats(projSpeed, projGravity, projImpactForce, projSoundLevel);
        }
        [MaintainOrder]
        [SettingName("Stats")]
        public ProjectileStats Stats;

        ///< @brief Contains the values to be applied to projectiles in this group.

        public string GetMatchlistAsString()
        {
            return Matchlist.Aggregate("Matchlist:[", (current, match) => current + (' ' + match.Name + (match.Required ? "[!] " : " "))) + " ]";
        }

        public string GetVarsAsString()
        {
            string str = "";
            str += "\t" + GetMatchlistAsString() + '\n';
            str += "\tSpeed:" + Stats.Speed + '\n';
            str += "\tGravity:" + Stats.Gravity + '\n';
            str += "\tImpactForce:" + Stats.ImpactForce + '\n';
            str += "\tSoundLevel:" + Stats.SoundLevel + '\n';
            return str;
        }
    }
}
