using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ClinicLogic.Models;

namespace Services.GiftServices.Models
{
    public class User
    {
        public string Id { get; set; }

        public List<Gift> gifts { get; set; }

        public Patient Patient { get; set; }

        public User(Patient patient) 
        {
            Patient = patient;
            Id = patient.CI;
            gifts = new List<Gift>();
        }
    }
}
