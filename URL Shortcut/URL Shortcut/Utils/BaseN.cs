namespace URL_Shortcut.Utils
{
    public class BaseN
    {
        public static string ChangeBase(long value, char[] charSet)
        {
            string result = string.Empty;
            uint targetBase = (uint)(charSet.Length);

            do
            {
                result = charSet[value % targetBase] + result;
                value = value / targetBase;
            }
            while (value > 0);

            return result;
        }
    }
}
