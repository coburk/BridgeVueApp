using System;

namespace BridgeVueApp.Models
{
    public class StudentProfile
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Grade { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public int GenderNumeric { get; set; }
        public string Ethnicity { get; set; }
        public int EthnicityNumeric { get; set; }
        public bool SpecialEd { get; set; }
        public bool IEP { get; set; }
        public bool HasKnownOutcome { get; set; }
        public bool? DidSucceed { get; set; }           
        public DateTime? CreatedDate { get; set; }      
        public DateTime? ModifiedDate { get; set; }
        public Guid LocalKey { get; set; } = Guid.NewGuid();
    }

}

