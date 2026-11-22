using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System;
using EdiParser.Models;
using EdiParser.Extensions;

namespace EdiParser.Parser
{
    public static class EDI834FileParser
    {
        public static void ParseSingleFile(
            string filePath,
            HashSet<string> targetSet,
            ConcurrentBag<Member834Record> results)
        {
            string sourceFile = Path.GetFileName(filePath);
            MemberState state = new MemberState();
            StringBuilder segmentBuilder = new StringBuilder();

            foreach (string line in File.ReadLines(filePath))
            {
                foreach (char c in line)
                {
                    if (c == '~')
                    {
                        EDISegmentProcessor.ProcessSegment(segmentBuilder, sourceFile, targetSet, results, state);
                        segmentBuilder.Clear();
                    }
                    else
                    {
                        segmentBuilder.Append(c);
                    }
                }
            }
            // Ensure the final member at the end of the file is safely evaluated and added
            FinalizeLastMember(sourceFile, results, state, segmentBuilder);
        }

        public static void FinalizeLastMember(
            string sourceFile,
            ConcurrentBag<Member834Record> results,
            MemberState state,
            StringBuilder segmentBuilder)
        {
            if (segmentBuilder.Length > 0 && state.HasCurrentMember && state.IsMatched)
            {
                results.Add(state.ToRecord());
            }
        }
    }
}
