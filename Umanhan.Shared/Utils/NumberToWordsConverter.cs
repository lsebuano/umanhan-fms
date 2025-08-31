using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umanhan.Shared.Utils
{
    public class NumberToWordsConverter
    {
        private static readonly string[] UnitsMap =
        {
        "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine",
        "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
        "Seventeen", "Eighteen", "Nineteen"
    };

        private static readonly string[] TensMap =
        {
        "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty",
        "Seventy", "Eighty", "Ninety"
    };

        public static string NumberToWords(long number)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += " ";

                if (number < 20)
                    words += UnitsMap[number];
                else
                {
                    words += TensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + UnitsMap[number % 10];
                }
            }

            return words.Trim();
        }

        public static string ConvertAmountToPesos(decimal amount)
        {
            long pesos = (long)Math.Floor(amount);
            int centavos = (int)((amount - pesos) * 100);

            string result = $"{NumberToWords(pesos)} pesos";

            if (centavos > 0)
            {
                result += $" and {NumberToWords(centavos)} centavos";
            }

            return result + " only";
        }
    }
}
