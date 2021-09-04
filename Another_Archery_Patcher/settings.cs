using System;
using System.Collections.Generic;
using System.Linq;
using Another_Archery_Patcher.ConfigHelpers;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher
{
    using static Flag.Type;
    using static Flag.State;
    
    public class Settings : StatsPreset
    {
        [MaintainOrder]
        [SettingName("Global Flag Tweaks")] // FLAG TWEAKS
        public List<FlagTweak> GlobalFlagTweaks = new() {
            new FlagTweak(Supersonic, Remove),
            new FlagTweak(DisableCombatAimCorrection, Add)
        };
        
        [SettingName("Game Settings"), Tooltip("Change the value of archery-related game settings.")] // GAME SETTINGS
        public GameSettings GameSettings = new(true, true, 8, 1F, 33);
        
        [Tooltip("Any projectiles listed here will not be added or modified.")] // BLACKLIST
        public List<FormLinkGetter<IProjectileGetter>> Blacklist = new() {
            Skyrim.Projectile.MQ101ArrowSteelProjectile,
        };
        
        private static readonly Dictionary<Projectile.TypeEnum, (bool, List<string>?)> ProjectileTypeWhitelist = new() {
            { Projectile.TypeEnum.Arrow, (true, null) },
            { Projectile.TypeEnum.Missile, (true, new List<string>{ "Trap" }) }
        };

        // FUNCTIONS
        
        // Check if a given formkey is on the blacklist
        private bool IsBlacklisted(FormKey projFormKey)
        {
            return Blacklist.Any(blacklisted => blacklisted.FormKey == projFormKey);
        }

        // Check if a given projectile is a valid target for the patcher
        public bool IsValidPatchTarget(IProjectileGetter projGetter)
        {
            if (projGetter.EditorID == null) 
                return false;
            foreach (var (_, (allow, matchlist)) in ProjectileTypeWhitelist.Where(proj => proj.Key == projGetter.Type))
                if (matchlist == null || matchlist.Any(str => projGetter.EditorID.Contains(str, StringComparison.OrdinalIgnoreCase))) 
                    return allow && !IsBlacklisted(projGetter.FormKey);
            return false;
        }
        
        // Apply the highest priority stats to a given projectile, if it isn't blacklisted and a valid category was found
        public (Projectile, uint, string) ApplyHighestPriorityStats(Projectile proj)
        {
            var stats = GetHighestPriorityStats(proj);
            var countChanges = 0u;
			if ( stats == null )
				return (proj, countChanges, "[NULL]");
			var identifier = stats!.Identifier;
            // Speed
            proj.Speed = stats.GetSpeed(proj.Speed, out var changed);
            countChanges += changed ? 1u : 0u;
            // Gravity
            proj.Gravity = stats.GetGravity(proj.Gravity, out changed);
            countChanges += changed ? 1u : 0u;
            // Impact Force
            proj.ImpactForce = stats.GetImpactForce(proj.ImpactForce, out changed);
            countChanges += changed ? 1u : 0u;
            // Sound Level
            proj.SoundLevel = stats.GetSoundLevel(proj.SoundLevel, out changed);
            countChanges += changed ? 1u : 0u;
            // Flags
            uint countFlagChanges;
            (proj, countFlagChanges) = Flag.Editor.ApplyFlags(proj, GlobalFlagTweaks); // apply global flags
            countChanges += countFlagChanges;
            (proj, countFlagChanges) = Flag.Editor.ApplyFlags(proj, stats.Flags); // apply specific flags
            countChanges += countFlagChanges;
            return (proj, countChanges, identifier);
        }
    }
}
