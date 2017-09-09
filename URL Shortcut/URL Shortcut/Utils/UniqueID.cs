using System;

namespace URL_Shortcut.Utils
{
    public class UniqueID
    {
        // Zero(0) implies failure
        public const ulong FAILURE = 0;

        public static ulong GetUniqueID(long initTime, long time, Random rnd)
        {
            // Make the number much smaller
            string d = (time - initTime).ToString();

            // Make it 14 digits
            while(d.Length<14)
            {
                d = d.Insert(0, "0");
            }

            // Pick 6 digits by random
            byte r1 = (byte)rnd.Next(0, 10);
            byte r2 = (byte)rnd.Next(0, 10);
            byte r3 = (byte)rnd.Next(0, 10);
            byte r4 = (byte)rnd.Next(0, 10);
            byte r5 = (byte)rnd.Next(0, 10);
            byte r6 = (byte)rnd.Next(0, 10);

            // Concatenate them all
            string y = string.Format("{0}{1}{2}{3}{4}{5}{6}", d, r1, r2, r3, r4, r5, r6);

            ulong z;

            // Try to parse the string
            try
            {
                z = ulong.Parse(y);
            }
            catch(OverflowException/* ex*/)
            {
                // It has exceeded the upper limit of unsigned long
                z = FAILURE;

                // Throw exception
                //ArithmeticException mathEx = new ArithmeticException("Unsigned Long Overflow.", ex);
                //throw mathEx;
            }

            // Return the number
            return z;
        }
    }
}
