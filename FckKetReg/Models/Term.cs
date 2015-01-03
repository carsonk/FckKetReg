using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FckKetReg.Models
{
    /// <summary>
    /// Model for a term.
    /// </summary>
    class Term
    {
        private int _year;
        private string _season;
        private string _termCode;

        public Term(int year, string englishSeasonName)
        {
            _year = year;
            _season = GetSeasonCodeByName(englishSeasonName);
            _termCode = _year.ToString() + _season;
        }

        public Term(string seasonSpaceYearString)
        {
            try {
                string[] parts = seasonSpaceYearString.Split(' ');
                _season = GetSeasonCodeByName(parts[0]);
                _year = Convert.ToInt32(parts[1]);
                _termCode = _year.ToString() + _season;
            } catch(IndexOutOfRangeException e)
            {
                throw new ArgumentException("Term must be in format: \"Summer 2015\"");
            }
        }

        public string GetTermCode()
        {
            return _year.ToString() + _season;
        }

        public static string GetSeasonCodeByName(string englishName)
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
