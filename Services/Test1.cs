using Base.Models;
using Base.Services;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;

namespace EarthApi.Services
{
    public class Test1
    {
        public string MaskPII(string s)
        {
            //check
            if (IsEmail(s))
                return MaskEmail(s);
            else if (IsPhone(s))
                return MaskPhone(s);
            else
                throw new Exception("Must Input Email or Phone.");
        }

        //判斷是否Email: 字串長度8-40, 內容為英文大小寫(轉小寫)、"."、@
        private bool IsEmail(string email)
        {
            return !(email.Length >= 8 && email.Length <= 40) ? false :
                Regex.IsMatch(email.ToLower(), @"^[a-z]+@[a-z]+\.[a-z]+$");
        }

        //判斷是否Phone: 字串長度10-20, 內容為數字、空白、+-()4種符號
        private bool IsPhone(string phone)
        {
            return !(phone.Length >= 10 && phone.Length <= 20) ? false :
                Regex.IsMatch(phone, @"^[\d()+\- ]+$");
        }
        private string MaskEmail(string email)
        {
            //轉換為小寫
            email = email.ToLower();

            //取得名稱部分和網域部分
            var atIndex = email.IndexOf('@');
            var name = email[..atIndex];
            var domain = email[(atIndex + 1)..];

            //name只包含大小寫英文(已轉小寫)
            if (!Regex.IsMatch(name, "^[a-z]+$"))
                throw new Exception("Email Name Should Have Only English Letters.");

            //domain只包含大小寫英文、"."(位置不在首尾)            
            if (!Regex.IsMatch(domain, @"^[a-z]+(\.[a-z]+)+$"))
                throw new Exception("Email Domain Should Have Only English Letters and Dot Sign (middle position).");

            //將名稱部分的中間字母替換為 "*"
            var nameLen = name.Length;
            if (nameLen >= 2)
                name = name[..1] + new string('*', 5) + name[(nameLen - 1)..];

            //組合處理後的 Email
            return name + "@" + domain;
        }
        private string MaskPhone(string phone)
        {
            //只休留數字
            var newPhone = Regex.Replace(phone, @"[^\d]", "");
            var newPhoneLen = newPhone.Length;
            if (!(newPhoneLen >= 10 && newPhoneLen <= 13))
                throw new Exception("Phone Length Without Seperator Should be 10-13");

            //取得國家代碼的位數
            var cntyCodeLen = newPhone.Length - 10;
            var phoneRight4 = newPhone[(newPhoneLen - 4)..];    //取右邊4碼

            // 格式化輸出的電話號碼
            return (cntyCodeLen == 0)
                ? $"***-***-{phoneRight4}"
                : $"+{new string('*', cntyCodeLen)}-***-***-{phoneRight4}";
        }

        public int MostWordsFound(string[] sentences)
        {
            //Constraints-1:
            if (!(sentences.Length >= 1 && sentences.Length <= 100))
                throw new Exception("Constraints: 1 <= sentences.length <= 100");

            int index = 0;
            int maxWords = 0;
            string pattern = "^[a-z ]+$";   //字串只能小寫英文和空白
            foreach (var sentence in sentences)
            {
                //方便嵌入字串
                var sentenceIdx = $"sentence[{index}]";

                //將句子按照空格拆分成單詞
                var words = sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                //Constraints-2:
                if (!(words.Length >= 1 && words.Length <= 100))
                    throw new Exception($"Constraints: 1 <= {sentenceIdx}.length <= 100");

                //Constraints-3:
                if (!Regex.IsMatch(sentence, pattern))
                    throw new Exception($"Constraints: consists only of lowercase English letters and ' ' for {sentenceIdx}");

                //Constraints-4:
                if (sentence != sentence.Trim())
                    throw new Exception($"Constraints: does not have leading or trailing spaces for {sentenceIdx}");

                //Constraints-5:
                if (sentence.IndexOf("  ") >= 0)
                    throw new Exception($"Constraints: All the words in {sentenceIdx} are separated by a single space");

                //取較大字數
                maxWords = Math.Max(maxWords, words.Length);
                index++;
            }

            return maxWords;
        }

    } //class
}
