using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using EdiParser.Models;
using EdiParser.Extensions;
using System;

namespace EdiParser.Parser.Segments
{

    internal static class Nm1SegmentHandler
    {
        public static void Handle(string[] elements, HashSet<string> targetSet, MemberState state)
        {
            if (elements.Length <= 1) return;
            string entityIdentifier = elements[1].Trim();

            if (!entityIdentifier.Equals("IL", StringComparison.OrdinalIgnoreCase) &&
                !entityIdentifier.Equals("M8", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (elements.Length > 3) state.LastName = elements[3].NormalizeString();
            if (elements.Length > 4) state.FirstName = elements[4].NormalizeString();

            if (elements.Length > 9)
            {
                string value = elements[9].NormalizeString();

                if (value != null)
                {
                    state.MemberId = value;
                    if (targetSet.Contains(value))
                    {
                        state.IsMatched = true;
                        if (state.MatchedValue == null) state.MatchedValue = value;
                        if (state.MatchType == null) state.MatchType = "NM109";
                    }
                }
            }
        }
    }
}