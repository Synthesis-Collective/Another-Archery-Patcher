using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Another_Archery_Patcher.ConfigHelpers
{
    public class Matchable
    {
        [MaintainOrder]
        [Tooltip("The name used to identify this category. Not used by the patcher.")]
        public string Identifier = "";
        [Tooltip("The highest-priority applicable category is always used.")]
        public int Priority;
        [Tooltip("List of words that must appear in a projectile's EditorID to be considered applicable. Leave empty to match all.")]
        public List<string> MatchList = new();

        public string GetMatchListAsString() // only used for verbose logging
        {
            string ret = "MatchList:\t";
            if (!MatchList.Any())
                return ret + "( ALL )";
            ret += "[ ";
            return MatchList.Aggregate(ret, (current, match) => current + (match + ";")) + " ]";
        }
        
        private bool HasMatch(string id) // categories with no elements will always match.
        {
            return !MatchList.Any() || MatchList.Any(match => id.Contains(match, StringComparison.OrdinalIgnoreCase));
        }

        public int GetPriority(string id) // retrieves the priority number if this instance is applicable to the given id.
        {
            if (HasMatch(id))
                return Priority;
            return -1;
        }
    }

}
