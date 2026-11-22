using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using EdiParser.Models;
using EdiParser.Extensions;

namespace EdiParser.Parser.Segments
{
    public static class RefSegmentHandler
    {
        public static void Handle(string[] elements, HashSet<string> targetSet, MemberState state)
        {
            if (elements.Length <= 2) return;
            string qualifier = elements[1].Trim();
            string value = elements[2].NormalizeString();

            if (value == null) return;

            switch (qualifier)
            {
                case "0F":
                case "1W":
                    state.MemberId = value;
                    EvaluateMatch(targetSet, state, qualifier, value);
                    break;
                case "SY":
                    state.Ssn = value;
                    EvaluateMatch(targetSet, state, qualifier, value);
                    break;
            }
        }
        private static void EvaluateMatch(HashSet<string> targetSet, MemberState state, string qualifier, string value)
        {
            if (targetSet.Contains(value))
            {
                state.IsMatched = true;
                if (state.MatchedValue == null) state.MatchedValue = value;
                if (state.MatchType == null) state.MatchType = "REF*" + qualifier;
            }
            return;
        }
    }
}