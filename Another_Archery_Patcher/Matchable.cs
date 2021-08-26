using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher
{
    /**
     * @class MatchableElement
     * @brief Represents a single entry in the matchlist.
     */
    public class MatchableElement
    {
        public MatchableElement(string name, bool required = true)
        {
            Name = name;
            Required = required;
        }
        [MaintainOrder]
        [Tooltip("This is the string to search for in each Projectile's Editor ID (Or Form ID)")]
        public string Name;
        [Tooltip("When true, a projectile is required to match this entry to be considered part of this category, even if it matches other non-required entries too.")]
        public bool Required;

        public bool IsMatch(string? id)
        {
            if (id == null) return false;
            return id == Name || id.Contains(Name, StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return Name;
        }
    }
    /**
     * @class Matchable
     * @brief Base class that allows disabling itself, and exposes a list of strings used for matching records against it.
     */
    public class Matchable
    {
        public Matchable(List<MatchableElement>? matchlist)
        {
            Matchlist = matchlist ?? new List<MatchableElement>();
        }

        private bool ContainsRequired(string? id)
        {
            return id != null && Matchlist.All(elem => !elem.Required || elem.Required && elem.IsMatch(id));
        }
        private bool Contains(string? id)
        {
            return id != null && Matchlist.Any(elem => elem.IsMatch(id)) && ContainsRequired(id);
        }

        private bool ContainsAll(string? id)
        {
            return id != null && Matchlist.All(elem => elem.IsMatch(id)) && ContainsRequired(id);
        }
        /**
         * @brief Checks if the given Editor ID or Form ID is a case-insensitive match for any of the strings in the matchlist.
         */
        public bool IsMatch(string? id)
        {
            if (id == null || Matchlist.Count <= 0) return false;
            return Contains(id);
        }

        public bool IsPerfectMatch(string? id)
        {
            if (id == null || Matchlist.Count <= 0) return false;
            return ContainsAll(id);
        }
        [MaintainOrder]
        [SettingName("Common Names"), JsonDiskName("matchlist"), Tooltip("(Don't change this unless you know what you're doing!) Used to resolve projectile type, as there is no other way to distinguish between arrows/bolts/other")]
        public List<MatchableElement> Matchlist;
    }
    /**
     * @class MatchableBlacklist
     * @brief Represents a list of Records that are used for the record blacklisting feature.
     */
    public class MatchableBlacklist : Matchable
    {
        /**
         * @brief Default constructor that takes a list of blacklisted IDs & list of blacklisted Record FormLinks
         */
        public MatchableBlacklist(bool enabled, List<MatchableElement> blacklistedIDs, List<IFormLinkGetter<IProjectileGetter>> blacklistedRecords) : base(blacklistedIDs)
        {
            Enabled = enabled;
            Record = blacklistedRecords;
        }

        [MaintainOrder]
        [Tooltip("Disabling this will disable this blacklist.")]
        public bool Enabled;
        [SettingName("Records")]
        public List<IFormLinkGetter<IProjectileGetter>> Record;
    }
}
