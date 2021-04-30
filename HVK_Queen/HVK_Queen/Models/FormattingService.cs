using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HVK_Queen.Models {
    public class FormattingService {

        public string DogSizeFormat(string size) {
            string newSize;
            if (size == "S")
                newSize = "Small";
            else if (size == "M")
                newSize = "Medium";
            else if (size == "L")
                newSize = "Large";
            else
                newSize = size;
            return newSize;
        }

        public string GenderFormat(string gender) {
            string newGender;
            if (gender == "M")
                newGender = "Male";
            else if (gender == "F")
                newGender = "Female";
            else
                newGender = gender;
            return newGender;
        }

        public string DisplayProvince(string pv) {
            if (pv == "QC") {
                return "Quebec";
            } else if (pv == "ON") {
                return "Ontario";
            } else {
                return pv;
            }
        }

        public string DisplayPostalCode(string pc) {
            if (pc == null) {
                return "";
            }

            return pc.Substring(0, 3) + " " + pc.Substring(3, 3);
        }

        public string PhoneFormat(string pNum) {
            if (pNum != null)
                return "(" + pNum.Substring(0, 3) + ")-" + pNum.Substring(3, 3) + "-" + pNum.Substring(6, 4);
            else
                return pNum;
        }

        public string DateFormat(DateTime date) {
            return date.ToString("yyyy/MM/dd");
        }

        public string CurrencyFormat(Double? amount) {
            return $"{amount:C}";
        }
    }
}
