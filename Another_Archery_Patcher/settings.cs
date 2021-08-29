using System.Collections.Generic;
using System.Linq;
using Another_Archery_Patcher.ConfigHelpers;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using SoundLevel = Another_Archery_Patcher.ConfigHelpers.SoundLevel;

namespace Another_Archery_Patcher
{
    public class Settings : StatsDefault
    {
        public Settings() // Default Constructor
        {
            // If disable gravity for bloodcursed arrows is enabled, add a new category to override arrows
            if (MiscTweaks.DisableGravityBloodcursed)
                ProjectileTweaks.Add(
                    new Stats("Bloodcursed Arrows", 
                        2, 
                        5000.0f, 
                        0f, 
                        0.44f, 
                        SoundLevel.Silent,
                        new List<string> { "DCL1ArrowElvenBloodProjectile", "DLC1AurielsBloodDippedProjectile" }
                    )
                );
        }
        
        [SettingName("Game Settings")]
        public GameSettings GameSettings = new(true, true);
        [SettingName("General Tweaks")]
        public MiscTweaks MiscTweaks = new(true, true, true);
        [Tooltip("Any projectiles listed here will not be patched.")]
        public List<FormLinkGetter<IProjectileGetter>> Blacklist = new()
        {
            Skyrim.Projectile.MQ101ArrowSteelProjectile,
        };
        [SettingName("Fun Tweaks"), Tooltip("Not meant for serious gameplay.")]
        public FunTweaks FunTweaks = new(true, true, true);
        [SettingName("Verbose Logging"), JsonDiskName("verbose-log"), Tooltip("Prints additional information to the console.")]
        public bool UseVerboseLog = true;
        
        // Check if a given projectile is on the blacklist
        public bool IsBlacklisted(IProjectileGetter proj)
        {
            return Blacklist.Any(blacklisted => blacklisted.FormKey == proj.FormKey);
        }
        
        // Apply the highest priority stats to a given projectile, if it isn't blacklisted and a valid category was found
        public Projectile ApplyHighestPriorityStats(Projectile proj, out uint countChanges, out string identifier)
        {
            identifier = "";
            countChanges = 0u;
            var stats = GetHighestPriorityStats(proj);
            if (stats == null) return proj; // return unmodified projectile if no stats object was received
            identifier = stats.Identifier;
            // Speed
            proj.Speed = stats.GetSpeed(proj.Speed, out var changedSpeed);
            countChanges += changedSpeed ? 1u : 0u;
            // Gravity
            proj.Gravity = stats.GetGravity(proj.Gravity, out var changedGravity);
            countChanges += changedGravity ? 1u : 0u;
            // Impact Force
            proj.ImpactForce = stats.GetImpactForce(proj.ImpactForce, out var changedImpactForce);
            countChanges += changedImpactForce ? 1u : 0u;
            // Sound Level
            proj.SoundLevel = stats.GetSoundLevel(proj.SoundLevel, out var changedSoundLevel);
            countChanges += changedSoundLevel ? 1u : 0u;
            // Flags
            if (MiscTweaks.DisableSupersonic) {
                FlagEditor.RemoveFlag(proj, Projectile.Flag.Supersonic, out var countFlagChanges);
                countChanges += countFlagChanges;
            }
            if (GameSettings.DisableAutoaim) {
                FlagEditor.AddFlag(proj, Projectile.Flag.DisableCombatAimCorrection, out var countFlagChanges);
                countChanges += countFlagChanges;
            }
            if (FunTweaks.Enabled) {
                FunTweaks.ApplyTweaks(proj, out var countFlagChanges);
                countChanges += countFlagChanges;
            }
            return proj;
        }
    }
}
