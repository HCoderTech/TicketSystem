using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ticket.Data
{
    public class TicketEntry
    {
        public string TicketID="";
        public string TicketNo="";
        public string Name="";
        public string PhoneNumber="";
        public string Email="";
        public string Address="";
        public TicketStatus Status=TicketStatus.New;
        public TicketPriority Priority=TicketPriority.Normal;
        public string Category="";
        public string Resolver="";
        public string Description = "";
        public DateTime EntryDate = DateTime.Now.Date;
        public DateTime DueDate = DateTime.Now.AddDays(7);

        public ScenarioResult IsValid()
        {
            ScenarioResult result = new ScenarioResult();
            result.Success = true;


            //TicketNo validation
            ScenarioResult ticketNoValidationResult = isTicketNoValid();
            consolidateMessage(result, ticketNoValidationResult);

            //Name validation
            ScenarioResult nameValidationResult = isNameValid();
            consolidateMessage(result, nameValidationResult);

            //PhoneNumber validation
            ScenarioResult phoneNumberValidationResult = isPhoneNumberValid();
            consolidateMessage(result, phoneNumberValidationResult);

            //Email validation
            ScenarioResult emailValidationResult = isEmailValid();
            consolidateMessage(result, emailValidationResult);

            return result;
        }

        private void consolidateMessage(ScenarioResult result, ScenarioResult specificResult)
        {
            result.Success = result.Success && specificResult.Success;
            if(!result.Success)
            result.ErrorMessage += specificResult.ErrorMessage + "\n";
        }

        private ScenarioResult isNameValid()
        {
            ScenarioResult result = new ScenarioResult();
            if(string.IsNullOrEmpty(Name) || 
                string.IsNullOrWhiteSpace(Name)
                )
            {
                result.ErrorMessage = "Valid Name not entered";
                return result;
            }
            result.Success = true;
            return result;
        }

        private ScenarioResult isTicketNoValid()
        {
            ScenarioResult result = new ScenarioResult();
            if (string.IsNullOrEmpty(TicketNo) ||
                string.IsNullOrWhiteSpace(TicketNo)
                )
            {
                result.ErrorMessage = "Valid Ticket Number not entered";
                return result;
            }
            result.Success = true;
            return result;
        }

        private ScenarioResult isPhoneNumberValid()
        {
            ScenarioResult result = new ScenarioResult();
            if (string.IsNullOrEmpty(PhoneNumber) ||
                string.IsNullOrWhiteSpace(PhoneNumber) ||
                !isPhoneNumber(PhoneNumber)
                )
            {
                result.ErrorMessage = "Valid Phone Number not entered";
                return result;
            }
            result.Success = true;
            return result;
        }

        private ScenarioResult isEmailValid()
        {
            ScenarioResult result = new ScenarioResult();
            if (!(string.IsNullOrEmpty(Email) ||
                string.IsNullOrWhiteSpace(Email)) &&
                !isEmail(Email)
                )
            {
                result.ErrorMessage = "Valid Email Address not entered";
                return result;
            }
            result.Success = true;
            return result;
        }

        private bool isPhoneNumber(string number)
        {
            return Regex.Match(number, @"^([0-9]{6,13})$").Success;
        }

        private bool isEmail(string email)
        {
            return Regex.Match(email,@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success;
        }

       
        
        public TicketEntry(){
            
        }
    }
}
