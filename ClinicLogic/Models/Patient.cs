using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicLogic.Models
{
    public class Patient
    {
        public string CI { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string BloodType { get; set; }

        public string? PatientID { get; set; } 
        public Patient(string ci, string name, string lastName)
        {
            CI = ci;
            Name = name;
            LastName = lastName;
            BloodType = RandomBloodType();
            
        }

        private string RandomBloodType()
        {
            string[] bloodTypes = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            Random random = new Random();
            return bloodTypes[random.Next(bloodTypes.Length)];
        }
    }
}
