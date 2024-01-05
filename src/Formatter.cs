using System;

public partial class Formatter
{
    public static string FormatBigNumber(int number)
    {
        if (number >= 1_000_000)
        {
            return string.Format("{0,6:0.000}M", Math.Round((double)number / 1000000, 3));
        }
        if (number >= 1_000)
        {
            return string.Format("{0,6:0.0}K", Math.Round((double)number / 1000, 1));
        }
        return string.Format("{0,7:0}", number) + "";
    }
}
