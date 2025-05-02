using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicLogic.Models
{
    internal class Patient
    {
        string[] bloodType = new string[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
        public int CI { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BloodType { get; set; }
        public Patient(int ci, string firstName, string lastName)
        {
            CI = ci;
            FirstName = firstName;
            LastName = lastName;
            BloodType = RandomBloodType();
        }

        // put randomized blood type
        public string RandomBloodType()
        {
            Random random = new Random();
            int index = random.Next(bloodType.Length);
            return bloodType[index];
        }
    }
}
