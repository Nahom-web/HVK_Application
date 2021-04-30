const $$ = sel => document.querySelector(sel);

let Phone = $$("#Phone");
let CellPhone = $$("#CellPhone");
let VeterinarianPhone = $$("#Veterinarian_Phone");
let EmergencyContactPhone = $$("#EmergencyContactPhone");


let petInResChange = document.querySelectorAll(".petInResChange");
let petServicesForRes = $$(".petServicesForRes");

// making phone number inputs look pretty


let stripPhoneNumber = (pNum) => {

    let newStr = "";

    for (let i = 0; i < pNum.length; i++) {
        if (!isNaN(pNum[i])) {
            newStr += pNum[i];
        }
    }

    return newStr;

}

let formatPhoneNumber = (str) => {

    let newPhone = "";

    switch (str.length) {
        case 1:
            // 8
            newPhone = "(" + str;
            str = "";
            break;

        case 2:
            // 81
            newPhone = "(" + str;
            str = "";
            break;

        case 3:
            // 819
            newPhone = "(" + str + ")";
            str = "";
            break;

        case 4:
            // 8191
            newPhone = "(" + str.substring(0, 3) + ")-" + str.substring(3);
            str = "";
            break;

        case 5:
            // 81912
            newPhone = "(" + str.substring(0, 3) + ")-" + str.substring(3);
            str = "";
            break;

        case 6:
            // 819121
            newPhone = "(" + str.substring(0, 3) + ")-" + str.substring(3) + "-";
            str = "";
            break;

        case 7:
            // 819 121 1
            newPhone = "(" + str.substring(0, 3) + ")-" + str.substring(3, 6) + "-" + str.substring(6);
            str = "";
            break;

        case 8:
            // 819 121 11
            newPhone = "(" + str.substring(0, 3) + ")-" + str.substring(3, 6) + "-" + str.substring(6);
            str = "";
            break;

        case 9:
            // 819 121 111
            newPhone = "(" + str.substring(0, 3) + ")-" + str.substring(3, 6) + "-" + str.substring(6);
            str = "";
            break;

        case 10:
            // 819 121 1111
            newPhone = "(" + str.substring(0, 3) + ")-" + str.substring(3, 6) + "-" + str.substring(6);
            str = "";
            break;
        default:
            newPhone = str;
            break;
    }

    return newPhone;
}
if (Phone != null && CellPhone != null && VeterinarianPhone != null && EmergencyContactPhone != null) {
    Phone.addEventListener('keyup', () => {
        Phone.value = formatPhoneNumber(stripPhoneNumber(Phone.value));
    });


    CellPhone.addEventListener('keyup', () => {
        CellPhone.value = formatPhoneNumber(stripPhoneNumber(CellPhone.value));
    });


    VeterinarianPhone.addEventListener('keyup', () => {
        VeterinarianPhone.value = formatPhoneNumber(stripPhoneNumber(VeterinarianPhone.value));
    });


    EmergencyContactPhone.addEventListener('keyup', () => {
        EmergencyContactPhone.value = formatPhoneNumber(stripPhoneNumber(EmergencyContactPhone.value));
    });
}



