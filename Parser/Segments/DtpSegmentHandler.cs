using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using EdiParser.Models;
using EdiParser.Extensions;

namespace EdiParser.Parser.Segments
{
    public static class DtpSegmentHandler
    {
        public static void Handle(string[] elements, HashSet<string> targetSet, MemberState state)
        {
            if (elements.Length <= 3) return;
            string qualifier = elements[1].Trim();
            string value = elements[3].NormalizeString();

            if (value == null) return;

            switch (qualifier)
            {
                case "336":
                    if (state.dtp336 == null) state.dtp336 = value; ;
                    return;
                case "348":
                    if (state.dtp348 == null) state.dtp348 = value; ;
                    return;
                case "349":
                    if (state.dtp349 == null) state.dtp349 = value; ;
                    return;
            }
        }
    }
}