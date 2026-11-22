using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EdiParser.Models;
using EdiParser.Extensions;
using EdiParser.Parser;

namespace EdiParser
{
    public static class Fast834Parser
    {
        public static List<Member834Record> ParseFiles(
            string[] filePaths,
            string[] targetIds,
            int maxDegreeOfParallelism)
        {
            if (filePaths == null)
            {
                throw new ArgumentNullException("filePaths");
            }

            if (targetIds == null)
            {
                throw new ArgumentNullException("targetIds");
            }

            var targetSet = new HashSet<string>(
                targetIds,
                StringComparer.OrdinalIgnoreCase);

            var resultBag = new ConcurrentBag<Member834Record>();

            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Math.Max(
                    1,
                    Math.Min(maxDegreeOfParallelism, filePaths.Length))
            };

            Parallel.ForEach(
                filePaths,
                options,
                filePath =>
                {
                    EDI834FileParser.ParseSingleFile(
                        filePath,
                        targetSet,
                        resultBag);
                });

            var results = new List<Member834Record>(resultBag);

            results.Sort(
                delegate (Member834Record left, Member834Record right)
                {
                    int fileComparison = StringComparer.OrdinalIgnoreCase.Compare(
                        left.SourceFile,
                        right.SourceFile);

                    if (fileComparison != 0)
                    {
                        return fileComparison;
                    }

                    return left.MemberOrdinal.CompareTo(
                        right.MemberOrdinal);
                });
            return results;
        }
    }
}