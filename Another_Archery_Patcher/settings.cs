using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System.Collections.Generic;

namespace Another_Archery_Patcher
{
    public enum SoundLevel // todo: figure out what integers are used for sound levels
    {
        [MaintainOrder]
        silent = 2,
        normal = 1,
        loud = 0,
        very_loud = 3,
    }
    public class GameSettings // game setting settings
    {
        [MaintainOrder]
        [Tooltip("Disable Auto-Aim")]
        public bool disable_autoaim = true;
        [Tooltip("[Experimental] Disable NPC Dodge to prevent the \"ninja dodge\" bug. May interfere with some combat mods.")]
        public bool disable_npcDodge = false;
        public GameSettings(bool disableAutoAim, bool disableNPCDodge)
        {
            disable_autoaim = disableAutoAim;
            disable_npcDodge = disableNPCDodge;
        }

        public List<string> to_string()
        {
            return new List<string>{ "[" + disable_autoaim.ToString() + "]\tDisable Auto-Aim", "[" + disable_npcDodge.ToString() + "]\tFix NPC Ninja Dodge bug" };
        }
    }
    public class GeneralTweaks // global projectile tweaks
    {
        [MaintainOrder]
        [Tooltip("Remove the supersonic flag from projectiles of this type. The supersonic flag removes sound from in-flight projectiles.")]
        public bool disable_supersonic;
        public GeneralTweaks(bool disableSupersonicFlag)
        {
            disable_supersonic = disableSupersonicFlag;
        }

        public List<string> to_string()
        {
            return new List<string>{ "[" + disable_supersonic.ToString() + "]\tRemove supersonic flag" };
        }
    }
    public class ProjectileTweaks // projectile tweaks
    {
        [MaintainOrder]
        [Tooltip("Toggle the tweaks in this section.")]
        public bool enabled;
        [Tooltip("The speed of this type of projectile. Controls projectile drop.")]
        public float speed;
        [Tooltip("The amount of gravity applied to this type of projectile. Controls projectile drop.")]
        public float gravity;
        [Tooltip("The amount of force imparted into objects hit by projectiles of this type.")]
        public float impactForce;
        [Tooltip("The amount of detectable noise produced by in-flight projectiles.")]
        public SoundLevel soundLevel;
        public ProjectileTweaks(bool enable, float proj_speed, float proj_gravity, float proj_impactForce, SoundLevel proj_soundLevel)
        {
            enabled = enable;
            speed = proj_speed;
            gravity = proj_gravity;
            impactForce = proj_impactForce;
            soundLevel = proj_soundLevel;
        }
        public List<string> to_string(string myName)
        {
            var ret = new List<string> { "[" + enabled.ToString() + "]\t" + myName.ToUpper() + " ENABLED" };
            if ( enabled )
                ret.AddRange(new string[]{ "[" + speed.ToString() + "]\tSpeed", "[" + gravity.ToString() + "]\tGravity", "[" + impactForce.ToString() + "]\tImpact Force", "[" + soundLevel.ToString() + "]\tSound Level" });
            return ret;
        }
    }
    public class ProjectileTweaks_Arrow : ProjectileTweaks // arrow-specific projectile tweak specialization
    {
        [Tooltip("Makes it easier/possible to hit the sun with the Dawnguard DLC's Bloodcursed Elven Arrows"), SettingName("Remove Gravity from Bloodcursed Elven Arrows")]
        public bool disable_gravity_bloodcursed = true;
        [Ignore]
        public string[] bloodcursed_id = { "DCL1ArrowElvenBloodProjectile", "DLC1AurielsBloodDippedProjectile" }; // Editor ID list of bloodcursed arrows
        public ProjectileTweaks_Arrow(bool enable, float proj_speed, float proj_gravity, float proj_impactForce, SoundLevel proj_soundLevel, bool bloodcursed_fix) : base(enable, proj_speed, proj_gravity, proj_impactForce, proj_soundLevel)
        {
            disable_gravity_bloodcursed = bloodcursed_fix;
        }

        public List<string> to_string()
        {
            var ret = to_string("Arrow Tweaks");
            ret.AddRange(new string[]{ "[" + disable_gravity_bloodcursed.ToString() + "]\tDisable Gravity on Bloodcursed Elven Arrows" });
            return ret;
        }
    }
    public class ProjectileTweaks_Bolt : ProjectileTweaks
    {
        [Tooltip("Makes the ballista trap projectiles fly straighter, faster, and hit much harder."), SettingName("Patch Dwemer Ballista")]
        public bool include_ballista_trap;
        public ProjectileTweaks_Bolt(bool enable, float proj_speed, float proj_gravity, float proj_impactForce, SoundLevel proj_soundLevel, bool includeBallistae) : base(enable, proj_speed, proj_gravity, proj_impactForce, proj_soundLevel)
        {
            include_ballista_trap = includeBallistae;
        }
        public List<string> to_string()
        {
            var ret = to_string("Bolt Tweaks");
            ret.AddRange(new string[] { "[" + include_ballista_trap.ToString() + "]\tInclude Dwemer Ballista Trap" });
            return ret;
        }
    }
    public class TopLevelSettings // main settings object
    {
        [MaintainOrder]
        [Tooltip("Changes Game Settings. (GMST)"), SettingName("[GMST] Game Settings")]
        public GameSettings GameSettings = new(true, false);
        [Tooltip("Tweaks that are applied to all projectiles."), SettingName("Universal Projectile Tweaks")]
        public GeneralTweaks GeneralTweaks = new(true);
        [Tooltip("Tweaks that are applied to Arrows."), SettingName("Arrow Tweaks")]
        public ProjectileTweaks_Arrow ArrowTweaks = new(true, 5000.0f, 0.34f, 0.44f, SoundLevel.silent, true);
        [Tooltip("Tweaks that are applied to Bolts."), SettingName("Bolt Tweaks")]
        public ProjectileTweaks_Bolt BoltTweaks = new(true, 5800.0f, 0.34f, 0.64f, SoundLevel.normal, true);
        [Tooltip("Tweaks that are applied to Throwable Weapons & Spears."), SettingName("Throwable Tweaks")]
        public ProjectileTweaks ThrowableTweaks = new(true, 2800.0f, 0.13f, 2.5f, SoundLevel.silent);
        [Tooltip("Projectiles in this list will be skipped without changing anything."), SettingName("Projectile Blacklist")]
        public List<IFormLinkGetter<IProjectileGetter>> blacklist = new();

        public List<string> to_string()
        {
            var arr = GameSettings.to_string();
            arr.AddRange(GeneralTweaks.to_string());
            arr.AddRange(ArrowTweaks.to_string());
            arr.AddRange(BoltTweaks.to_string());
            arr.AddRange(ThrowableTweaks.to_string("Throwable Tweaks"));
            return arr;
        }
    }
}