namespace EdiParser.Extensions
{

    public static class StringExtensions
    {
        public static string NormalizeString(this string value)
        {
            if (value == null)
            {
                return null;
            }

            value = value.Trim();

            return value.Length == 0 ? null : value;
        }
    }
}