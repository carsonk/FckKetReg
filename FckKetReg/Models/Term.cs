using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FckKetReg.Models
{
    class Term
    {
        private int _year;
        private string _season;
        private string _termCode;

        public Term(int year, string englishSeasonName)
        {
            _year = year;
            _season = getSeasonCodeByName(englishSeasonName);
            _termCode = _year.ToString() + _season;
        }

        public Term(string termCode)
        {
            _termCode = termCode;
        }

        public string getTermCode()
        {
            return _year.ToString() + _season;
        }

        public static string getSeasonCodeByName(string englishName)
        {
            // Since they're throwing returns, they don't need breaks.
            switch (englishName)
            {
                case "Winter": return "01";
                case "Spring": return "02";
                case "Summer": return "03";
                case "Fall": return "04";
                default: return null;
            }
        }
    }
}
