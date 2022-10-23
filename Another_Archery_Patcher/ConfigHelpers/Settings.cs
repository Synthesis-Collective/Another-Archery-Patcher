using Another_Archery_Patcher.ConfigHelpers.RecordEditor;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Another_Archery_Patcher.ConfigHelpers
{
    public class Settings : StatsPresets
    {
        [MaintainOrder]
        [SettingName("Global Flag Tweaks")] // FLAG TWEAKS
        public EnumFlagSetting<Editor.Flag.Type> GlobalFlagTweaks = new()
        {
            FlagChanges = new() { new(EnumFlagOperationType.Disable, Editor.Flag.Type.Supersonic) }
        };

        [SettingName("Game Settings")]
        [Tooltip("Change the value of archery-related game settings.")] // GAME SETTINGS
        public GameSettings GameSettings = new(true, true, 8, 1F, 33, 12288);

        [Tooltip("Any projectiles listed here will not be added or modified.")] // BLACKLIST
        public List<FormLinkGetter<IProjectileGetter>> Blacklist = new()
        {
            Skyrim.Projectile.MQ101ArrowSteelProjectile,
        };

        private static readonly Dictionary<Projectile.TypeEnum, (bool, List<string>?)> ProjectileTypeWhitelist = new()
        {
            { Projectile.TypeEnum.Arrow, (true, null) },
            { Projectile.TypeEnum.Missile, (true, new List<string> { "Trap" }) }
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
            {
                if (matchlist?.Any(str => projGetter.EditorID.Contains(str, StringComparison.OrdinalIgnoreCase)) != false)
                    return allow && !IsBlacklisted(projGetter.FormKey);
            }
            return false;
        }

        // Apply the highest priority stats to a given projectile, if it isn't blacklisted and a valid category was found
        public (Projectile, uint, string) ApplyHighestPriorityStats(Projectile proj)
        {
            var stats = GetHighestPriorityStats(proj);
            var countChanges = 0u;
            if (stats == null)
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
            proj.Flags = (Projectile.Flag)GlobalFlagTweaks.GetValueOrAlternative((Editor.Flag.Type)proj.Flags, out changed);
            if (changed)
                countChanges++;

            return (proj, countChanges, identifier);
        }
    }
}
