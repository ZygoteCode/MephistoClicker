using System;
using System.Collections.Generic;

public static class Utils
{
    public static string ConvertKeysToString(System.Windows.Forms.Keys key)
    {
        return key.ToString();
    }

    public static System.Windows.Forms.Keys ConvertStringToKeys(string key)
    {
        return (System.Windows.Forms.Keys)Enum.Parse(typeof(System.Windows.Forms.Keys), key);
    }

    public static IEnumerable<string> SplitToLines(string input)
    {
        if (input == null)
        {
            yield break;
        }

        using (System.IO.StringReader reader = new System.IO.StringReader(input))
        {
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}