namespace EdiParser.Models
{
    public class MemberState
    {
        // Tracking & Control Flags
        public bool HasCurrentMember { get; set; }
        public int MemberOrdinal { get; set; }
        public bool IsMatched { get; set; }
        public string MatchedValue { get; set; }
        public string MatchType { get; set; }

        // Member Demographics
        public string SourceFile { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MemberId { get; set; }
        public string Ssn { get; set; }
        public string Dob { get; set; }
        public string dtp336 { get; set; }
        public string dtp348 { get; set; }
        public string dtp349 { get; set; }

        // Reset state when hitting a new member segment (e.g., INS segment)
        public void Reset(string sourceFile, int ordinal)
        {
            SourceFile = sourceFile;
            MemberOrdinal = ordinal;
            HasCurrentMember = true;
            IsMatched = false;
            MatchedValue = null;
            MatchType = null;
            LastName = null;
            FirstName = null;
            MemberId = null;
            Ssn = null;
            Dob = null;
            dtp336 = null;
            dtp348 = null;
            dtp349 = null;
        }

        public Member834Record ToRecord()
        {
            return new Member834Record
            {
                SourceFile = SourceFile,
                MemberOrdinal = MemberOrdinal,
                MemberId = MemberId,
                Ssn = Ssn,
                Dob = Dob,
                dtp336 = dtp336,
                dtp348 = dtp348,
                dtp349 = dtp349,
                MatchedValue = MatchedValue,
                MatchType = MatchType
            };
        }

        // This encapsulates the mapping once so you never repeat it
        public Member834Record ToRecord(string matchedValue, string matchType)
        {
            return new Member834Record
            {
                SourceFile = SourceFile,
                MemberOrdinal = MemberOrdinal,
                LastName = LastName,
                FirstName = FirstName,
                MemberId = MemberId,
                Ssn = Ssn,
                Dob = Dob,
                dtp336 = dtp336,
                dtp348 = dtp348,
                dtp349 = dtp349,
                MatchedValue = matchedValue,
                MatchType = matchType
            };
        }
    }
}