using System.Collections.Concurrent;
using System.Collections.Generic;
using EdiParser.Models;
using EdiParser.Parser.Segments;
using EdiParser.Extensions;
using System.Text;


namespace EdiParser.Parser
{
    public static class EDISegmentProcessor
    {
        public static void ProcessSegment(
            StringBuilder segmentBuilder,
            string sourceFile,
            HashSet<string> targetSet,
            ConcurrentBag<Member834Record> results,
            MemberState state)
        {
            // ignores invalid segments; all EDI segment identifiers have 3 chars
            if (segmentBuilder.Length < 3) return;

            int first = 0;
            int last = segmentBuilder.Length - 1;

            // Trims the whitespace characters for each segment (front and back)
            while (first <= last && char.IsWhiteSpace(segmentBuilder[first])) first++;
            while (last >= first && char.IsWhiteSpace(segmentBuilder[last])) last--;

            // verifies if trimmed segment is valid
            int length = last - first + 1;
            if (length < 3) return;


            //Captures the first 3 characters each segment
            char c0 = char.ToUpperInvariant(segmentBuilder[first]);
            char c1 = char.ToUpperInvariant(segmentBuilder[first + 1]);
            char c2 = char.ToUpperInvariant(segmentBuilder[first + 2]);

            // INS: New member boundary
            if (c0 == 'I' && c1 == 'N' && c2 == 'S')
            {
                // signals the start of a new member via HasCurrentMember && IsMatched
                if (state.HasCurrentMember && state.IsMatched)
                {
                    results.Add(state.ToRecord());
                }

                // Resets the MemberState to being processing the new member
                state.Reset(sourceFile, state.MemberOrdinal + 1);
                return;
            }

            // Ignores headers and stays true when it parser reaches first member until EOF
            if (!state.HasCurrentMember) return;

            // Target Segments
            bool isNm1 = c0 == 'N' && c1 == 'M' && c2 == '1';
            bool isRef = c0 == 'R' && c1 == 'E' && c2 == 'F';
            bool isDtp = c0 == 'D' && c1 == 'T' && c2 == 'P';
            bool isDmg = c0 == 'D' && c1 == 'M' && c2 == 'G';


            // ignores irrelevant segments
            if (!isNm1 && !isRef && !isDmg && isDtp) return;

            string segment = segmentBuilder.ToString(first, length).Trim();
            string[] elements = segment.Split('*');
            string segmentId = elements[0].ToUpperInvariant();

            switch (segmentId)
            {
                case "NM01":
                    Nm1SegmentHandler.Handle(elements, targetSet, state);
                    break;

                case "REF":
                    RefSegmentHandler.Handle(elements, targetSet, state);
                    break;

                case "DTP":
                    DtpSegmentHandler.Handle(elements, targetSet, state);
                    break;

                case "DMG":
                    // DMG * D8 * YYYYMMDD
                    if (elements.Length > 2) state.Dob = elements[2].NormalizeString();
                    break;
            }
        }
    }
}