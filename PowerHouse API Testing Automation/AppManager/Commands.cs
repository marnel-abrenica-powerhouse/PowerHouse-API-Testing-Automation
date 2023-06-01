using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerHouse_API_Testing_Automation.AppManager
{
    public class Commands
    {
        public string StringGenerator(string type = "allletters", int digit = 9)
        {
            Random ranInt = new();
            var seedInt = ranInt.Next();
            Random rand = new(seedInt);
            string allowedChars = "";

            switch (type)
            {
                case "alphanumeric":
                    allowedChars = "abcdefghijklmnopqrstuvwxyz0123456789";
                    break;
                case "alphanumeric+SpecialChar":
                    allowedChars = "abcdefghijklmnopqrstuvwxyz0123456789!()-@_\"#$%&'*+,./;:<=>?[|]~ ";
                    break;
                case "specialcharacters":
                    allowedChars = "!()-@_\"#$%&'*+,./;:<=>?[|]~ ";
                    break;
                case "allletters":
                    allowedChars = "abcdefghijklmnopqrstuvwxyz";
                    break;
                case "allletters+SpecialChar":
                    allowedChars = "abcdefghijklmnopqrstuvwxyz!()-@_\"#$%&'*+,./;:<=>?[|]~ ";
                    break;
                case "allnumbers":
                    allowedChars = "0123456789";
                    break;
                default:
                    allowedChars = "abcdefghijklmnopqrstuvwxyz0123456789";
                    break;
            }


            var newString = char.ToUpper(new string(Enumerable.Repeat(allowedChars, digit)
                .Select(s => s[rand.Next(s.Length)]).ToArray())[0]) + new string(Enumerable.Repeat(allowedChars, digit)
                .Select(s => s[rand.Next(s.Length)]).ToArray()).Substring(1);
            return newString;
        }



    }
}
