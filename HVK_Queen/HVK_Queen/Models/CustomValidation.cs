using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HVK_Queen.Models {
    public class CustomValidation {

        // Reservation Fields:
        public sealed class CheckStartDate : ValidationAttribute {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
                if (value != null) {
                    if ((DateTime)value < DateTime.Now.Date) {
                        return new ValidationResult("Start Date Must be today's date or in the future");
                    } else if ((DateTime)value > DateTime.Now.AddMonths(8)) {
                        return new ValidationResult("Sorry, you can't make a reservation 8 months or more in advance.");
                    } else {
                        return ValidationResult.Success;
                    }
                } else {
                    return new ValidationResult("Please enter in a date");
                }
            }

        } // CheckStartDate validation class

        public sealed class CheckEndDate : ValidationAttribute {

            public string StartDate { get; set; }

            public CheckEndDate(string startDate) {
                StartDate = startDate;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
                var startDateInfo = validationContext.ObjectType.GetProperty(this.StartDate);
                var startDateValue = startDateInfo.GetValue(validationContext.ObjectInstance, null);

                if (value != null) {
                    if ((DateTime)value <= (DateTime)startDateValue) {
                        return new ValidationResult("Please enter an end date that is after the start date.");
                    } else if ((DateTime)value > ((DateTime)startDateValue).AddDays(28)) {
                        return new ValidationResult("Please change your end date so your reservation is under 4 weeks.");
                    } else {
                        return ValidationResult.Success;
                    }
                } else {
                    return new ValidationResult("Please enter in a date");
                }
            }

        } // CheckEndDate validation class

        public sealed class ValidateMedication : ValidationAttribute {

            public string Medication { get; set; }
            public string Name { get; set; }
            public string EndDate { get; set; }
            public string StartDate { get; set; }

            public ValidateMedication(string medication, string name, string endDate, string startDate) {
                Medication = medication;
                Name = name;
                EndDate = endDate;
                StartDate = startDate;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
                var med = validationContext.ObjectType.GetProperty(this.Medication);
                var medValue = med.GetValue(validationContext.ObjectInstance, null);

                var name = validationContext.ObjectType.GetProperty(this.Name);
                var nameValue = name.GetValue(validationContext.ObjectInstance, null);

                var endDate = validationContext.ObjectType.GetProperty(this.EndDate);
                var endDateValue = endDate.GetValue(validationContext.ObjectInstance, null);

                var resStart = validationContext.ObjectType.GetProperty(this.StartDate);
                var resStartDateValue = resStart.GetValue(validationContext.ObjectInstance, null);

                if ((bool)medValue == true) {
                    if (value == nameValue) {
                        if (value != null) {
                            return ValidationResult.Success;
                        } else {
                            return new ValidationResult("Please enter your pet's medication name.");
                        }
                    } else if (value == endDateValue) {
                        if ((DateTime)value != null) {
                            if ((DateTime)value < (DateTime)resStartDateValue) {
                                return new ValidationResult($"Please double check expiry date, it should not be before start date: {((DateTime)resStartDateValue).ToString("MMMM dd, yyyy")}");
                            } else {
                                return ValidationResult.Success;
                            }
                        } else {
                            return new ValidationResult("Please select your medication end date.");
                        }
                    } else {
                        return ValidationResult.Success;
                    }
                } else {
                    return ValidationResult.Success;
                }

            }

        } // ValidateMedication validation class

        // Owner Fields:
        public sealed class CheckEmailAndPhoneFields : ValidationAttribute {

            public string Email { get; set; }
            public string Phone { get; set; }
            public string CellPhone { get; set; }

            public CheckEmailAndPhoneFields(string v1, string v2, string v3) {
                this.Email = v1;
                this.Phone = v2;
                this.CellPhone = v3;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
                var email = validationContext.ObjectType.GetProperty(this.Email);
                var emailValue = email.GetValue(validationContext.ObjectInstance, null);

                var pNum = validationContext.ObjectType.GetProperty(this.Phone);
                var pNumValue = pNum.GetValue(validationContext.ObjectInstance, null);

                var cNum = validationContext.ObjectType.GetProperty(this.CellPhone);
                var cNumValue = cNum.GetValue(validationContext.ObjectInstance, null);

                // if all fields are empty
                if (emailValue == null && pNumValue == null && cNumValue == null) {
                    return new ValidationResult("Please enter email, phone number or cell phone number");
                } else {
                    return ValidationResult.Success;
                }

            }
        } // CheckEmailAndPhoneFields validation class

        public sealed class ValidateVeterinarianInformation : ValidationAttribute {
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string PostalCode { get; set; }

            public ValidateVeterinarianInformation(string name, string phone, string street, string city, string postalCode) {
                Name = name;
                Phone = phone;
                Street = street;
                City = city;
                PostalCode = postalCode;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
                var Name = validationContext.ObjectType.GetProperty(this.Name);
                var NameValue = Name.GetValue(validationContext.ObjectInstance, null);

                var Phone = validationContext.ObjectType.GetProperty(this.Phone);
                var PhoneValue = Phone.GetValue(validationContext.ObjectInstance, null);

                var StreetI = validationContext.ObjectType.GetProperty(this.Street);
                var StreetValue = StreetI.GetValue(validationContext.ObjectInstance, null);

                var CityI = validationContext.ObjectType.GetProperty(this.City);
                var CityValue = CityI.GetValue(validationContext.ObjectInstance, null);

                var PostalCodeI = validationContext.ObjectType.GetProperty(this.PostalCode);
                var PostalCodeValue = PostalCodeI.GetValue(validationContext.ObjectInstance, null);

                if (value == NameValue) {
                    if ((StreetValue != null || CityValue != null || PostalCodeValue != null) && value == null) {
                        return new ValidationResult("Please enter the Phone Number of your Veterinarian");
                    } else if (NameValue == null && value == null) {
                        return ValidationResult.Success;
                    } else if (NameValue != null && value == null) {
                        return new ValidationResult("Please enter the Phone Number of your Veterinarian");
                    } else {
                        return ValidationResult.Success;
                    }
                } else if (value == PhoneValue) {
                    if ((StreetValue != null || CityValue != null || PostalCodeValue != null) && value == null) {
                        return new ValidationResult("Please enter the Name of your Veterinarian");
                    } else if (PhoneValue == null && value == null) {
                        return ValidationResult.Success;
                    } else if (PhoneValue != null && value == null) {
                        return new ValidationResult("Please enter the Name of your Veterinarian");
                    } else {
                        return ValidationResult.Success;
                    }
                } else {
                    return ValidationResult.Success;
                }
            }
        } // ValidateVeterinarianInformation validation class

        public sealed class ValidateEmergencyContact : ValidationAttribute {

            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Phone { get; set; }

            public ValidateEmergencyContact(string firstName, string lastName, string phoneNumber) {
                FirstName = firstName;
                LastName = lastName;
                Phone = phoneNumber;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
                var FName = validationContext.ObjectType.GetProperty(this.FirstName);
                var FNameValue = FName.GetValue(validationContext.ObjectInstance, null);

                var LName = validationContext.ObjectType.GetProperty(this.LastName);
                var LNameValue = LName.GetValue(validationContext.ObjectInstance, null);

                var PNum = validationContext.ObjectType.GetProperty(this.Phone);
                var PNumValue = PNum.GetValue(validationContext.ObjectInstance, null);


                if (FNameValue == null && LNameValue == null && PNumValue == null) {
                    return ValidationResult.Success;
                } else {
                    if (value == FNameValue) {
                        if ((LNameValue != null || PNumValue != null) && value == null) {
                            return new ValidationResult("Please enter first name for emergency contact");
                        } else {
                            return ValidationResult.Success;
                        }
                    } else if (value == LNameValue) {
                        if ((FNameValue != null || PNumValue != null) && value == null) {
                            return new ValidationResult("Please enter last name for emergency contact");
                        } else {
                            return ValidationResult.Success;
                        }
                    } else if (value == PNumValue) {
                        if ((FNameValue != null || LNameValue != null) && value == null) {
                            return new ValidationResult("Please enter phone number for emergency contact");
                        } else {
                            return ValidationResult.Success;
                        }
                    } else {
                        return ValidationResult.Success;
                    }
                }
            }
        } // ValidateEmergencyContact validation class

        public sealed class CheckBirthday : ValidationAttribute {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
                if (value != null) {
                    if (value.ToString().Length == 4) {
                        try {
                            var year = Convert.ToInt32(value.ToString().Substring(0, 4));
                            if (year < 2000 || year > 2020)
                                return new ValidationResult("Year must be between 2000 and 2020.");
                            else
                                return ValidationResult.Success;
                        } catch (FormatException) {
                            return new ValidationResult("Date Format is not right,should: yyyy");
                        }
                    } else if (value.ToString().Length == 5) {
                        try {
                            var month = Convert.ToInt32(value.ToString().Substring(0, 2));
                            var year = Convert.ToInt32(value.ToString().Substring(3, 2));
                            if (year < 00 || year > 20 || month < 1 || month > 12)
                                return new ValidationResult("Format:mm-yy,Month between 1 and 12,Year between 00 and 20.");
                            else
                                return ValidationResult.Success;
                        } catch (FormatException) {
                            return new ValidationResult("Date Format is not right,should: mm-yy");
                        }
                    } else if (value.ToString().Length == 7) {
                        try {
                            var month = Convert.ToInt32(value.ToString().Substring(0, 2));
                            var year = Convert.ToInt32(value.ToString().Substring(3, 4));
                            if (year < 2000 || year > 2020 || month < 1 || month > 12)
                                return new ValidationResult("Format:mm-yyyy,Month between 1 and 12,Year between 2000 and 2020.");
                            else
                                return ValidationResult.Success;
                        } catch (FormatException) {
                            return new ValidationResult("Date Format is not right,should: mm-yyyy");
                        }
                    } else if (value.ToString().Length == 8) {
                        try {
                            var day = Convert.ToInt32(value.ToString().Substring(0, 2));
                            var month = Convert.ToInt32(value.ToString().Substring(3, 2));
                            var year = Convert.ToInt32(value.ToString().Substring(6, 2));
                            if (year < 00 || year > 20 || month < 1 || month > 12 || day < 1 || day > 31)
                                return new ValidationResult("Format:dd-mm-yy,Day between 1 and 31, Month between 1 and 12,Year between 00 and 20.");
                            else
                                return ValidationResult.Success;
                        } catch (FormatException) {
                            return new ValidationResult("Date Format is not right,should: dd-mm-yy");
                        }
                    } else if (value.ToString().Length == 10) {
                        try {
                            var day = Convert.ToInt32(value.ToString().Substring(0, 2));
                            var month = Convert.ToInt32(value.ToString().Substring(3, 2));
                            var year = Convert.ToInt32(value.ToString().Substring(6, 4));
                            if (year < 2000 || year > 2020 || month < 1 || month > 12 || day < 1 || day > 31)
                                return new ValidationResult("Format:dd-mm-yyyy,Day between 1 and 31, Month between 1 and 12,Year between 2000 and 2020.");
                            else
                                return ValidationResult.Success;
                        } catch (FormatException) {
                            return new ValidationResult("Date Format is not right,should: dd-mm-yyyy");
                        }
                    } else {
                        return new ValidationResult("Date format is not right.");
                    }
                } else {
                    return ValidationResult.Success;
                }
            }
        }
    } // CustomValidation class
} // HVK_Queen.Models namespace