using Another_Archery_Patcher.ConfigHelpers;
using Another_Archery_Patcher.ConfigHelpers.RecordEditor;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Another_Archery_Patcher
{
    using static ProjectileFlag.State;
    using static ProjectileFlag.Flag;

    /// <summary>
    /// Top-level settings object that stores all of the patcher's settings.
    /// </summary>
    public class Settings : StatsPresets
    {
        [MaintainOrder]
        [SettingName("Global Flag Tweaks")] // FLAG TWEAKS
        public FlagTweakList GlobalFlagTweaks = new()
        {
            new(Supersonic, Remove),
        };

        [SettingName("Game Settings"), Tooltip("Change the value of archery-related game settings.")] // GAME SETTINGS
        public GameSettings GameSettings = new(true, true, 8, 1F, 33);

        [Tooltip("Any projectiles listed here will not be added or modified.")] // BLACKLIST
        public List<FormLinkGetter<IProjectileGetter>> Blacklist = new()
        {
            Skyrim.Projectile.MQ101ArrowSteelProjectile,
        };

        /// <summary>
        /// Used to determine which projectile types the patcher supports.
        /// This is not exposed to the end user.
        /// </summary>
        private static readonly Dictionary<Projectile.TypeEnum, (bool, List<string>?)> ProjectileTypeWhitelist = new()
        {
            { Projectile.TypeEnum.Arrow, (true, null) },
            { Projectile.TypeEnum.Missile, (true, new List<string> { "Trap" }) }
        };

        // FUNCTIONS

        /// <summary>
        /// Check if the given <see cref="FormKey"/> is present on the blacklist.
        /// </summary>
        /// <param name="projFormKey">FormKey to check.</param>
        /// <returns><list type="table">
        /// <item><term>true</term><description>Given FormKey is blacklisted, and should be ignored.</description></item>
        /// <item><term>false</term><description>Given FormKey isn't blacklisted.</description></item>
        /// </list></returns>
        private bool IsBlacklisted(FormKey projFormKey)
        {
            return Blacklist.Any(blacklisted => blacklisted.FormKey == projFormKey);
        }

        /// <summary>
        /// Check if the given <see cref="IProjectileGetter"/> is a valid target for the patcher by checking the <see cref="FormKey"/> blacklist.
        /// </summary>
        /// <param name="projGetter">Projectile record getter to check.</param>
        /// <returns><list type="table">
        /// <item><term>true</term><description>Projectile record should be processed.</description></item>
        /// <item><term>false</term><description>Projectile record should be ignored.</description></item>
        /// </list></returns>
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

        /// <summary>
        /// Apply the stats from the projectile category with the highest priority.
        /// </summary>
        /// <param name="proj">A <see cref="Projectile"/> record to modify.</param>
        /// <returns>A tuple with the following:
        /// <list type="table">
        /// <item><term>Item1</term><description>The modified <see cref="Projectile"/> record.</description></item>
        /// <item><term>Item2</term><description>The number of changed subrecords.</description></item>
        /// <item><term>Item3</term><description>The identifier of the stats category that was applied to the projectile.</description></item>
        /// </list></returns>
        public (Projectile, uint, string) ApplyHighestPriorityStats(Projectile proj)
        {
            if (proj.EditorID == null)
                return (proj, 0, "");
            var stats = GetHighestPriorityStats(proj.EditorID);
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
            proj = GlobalFlagTweaks.ApplyTo(proj, out uint globalFlagChanges);
            countChanges += globalFlagChanges;
            proj = stats.Flags.ApplyTo(proj, out uint statFlagChanges);
            countChanges += statFlagChanges;

            return (proj, countChanges, identifier);
        }
    }
}
