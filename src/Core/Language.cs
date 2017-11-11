namespace Core
{
    public class Language
    {
        public const int EnglishId = 1;

        public const string English = "en";
        public const string Russian = "ru";
        public const string Ukrainian = "uk";

        public static int? GetLanguageId(string language)
        {
            if (string.Equals(English, language.Trim().ToLower()))
                return EnglishId;

            if (string.Equals(Russian, language.Trim().ToLower()))
                return 2;

            if (string.Equals(Ukrainian, language.Trim().ToLower()))
                return 3;

            return null;
        }
    }
}