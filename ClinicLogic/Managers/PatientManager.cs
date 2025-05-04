using ClinicLogic.Models;

namespace ClinicLogic.Managers
{
    public class PatientManager
    {
        private List<Patient> listPatients = new List<Patient>();

        public PatientManager()
        {
            listPatients = new List<Patient>()
            {
                new Patient(1, "John", "Doe"),
                new Patient(2, "Jane", "Smith"),
                new Patient(3, "Alice", "Johnson"),
            };
        }

        public List<Patient> GetPatients()
        {
            return listPatients;
        }

        public void AddPatient(Patient patient)
        {
            if (listPatients.Any(p => p.CI == patient.CI))
            {
                throw new Exception("A patient with the same CI already exists.");
            }
            listPatients.Add(patient);
        }
        public void UpdatePatient(int ci, Patient updatedPatient)
        {
            var patient = listPatients.FirstOrDefault(p => p.CI == ci);
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }
            patient.FirstName = updatedPatient.FirstName;
            patient.LastName = updatedPatient.LastName;
        }
        public void DeletePatient(int ci)
        {
            var patient = listPatients.FirstOrDefault(p => p.CI == ci);
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }
            listPatients.Remove(patient);
        }
        public Patient GetPatient(int ci)
        {
            var patient = listPatients.FirstOrDefault(p => p.CI == ci);
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }
            return patient;
        }

    }
}
