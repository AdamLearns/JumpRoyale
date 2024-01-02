using System;

public partial class Formatter
{	public static string FormatBigNumber(int number) {
        if (number >= 1000000) {
            return string.Format("{0,6:0.00}M", Math.Round((double)number/1000000, 2));
        }
        if (number >= 1000) {
            return string.Format("{0,6:0.0}K", Math.Round((double)number/1000, 1));
        }  
        return string.Format("{0,7:0}", number) + "";
    }
    
}
