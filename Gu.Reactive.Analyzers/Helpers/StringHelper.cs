namespace Gu.Reactive.Analyzers
{
    internal static class StringHelper
    {
        internal static string ToFirstCharUpper(this string text)
        {
            if (char.IsLower(text[0]))
            {
                return new string(char.ToUpper(text[0]), 1) + text.Substring(1);
            }

            return text;
        }
    }
}
