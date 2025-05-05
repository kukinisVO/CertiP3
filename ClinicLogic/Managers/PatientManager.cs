using ClinicLogic.Models;
using Microsoft.Extensions.Options;

namespace ClinicLogic.Managers
{
    public class PatientManager
    {
        private readonly AppConfig _config;

        public PatientManager(IOptions<AppConfig> config)
        {
            _config = config.Value;
        }

        public string GetFilePath() => _config.PatientsFilePath;
        // Helper: Read all patients using StreamReader
        private List<Patient> ReadAllPatients()
        {
            var patients = new List<Patient>();

            using (var reader = new StreamReader(_config.PatientsFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');
                    patients.Add(new Patient(parts[2], parts[0], parts[1])
                    {
                        BloodType = parts[3]
                    });
                }
            }

            return patients;
        }

        // Helper: Save all patients using StreamWriter
        private void SaveAllPatients(List<Patient> patients)
        {
            using (var writer = new StreamWriter(_config.PatientsFilePath))
            {
                foreach (var patient in patients)
                {
                    writer.WriteLine($"{patient.Name},{patient.LastName},{patient.CI},{patient.BloodType}");
                }
            }
        }

        // CREATE
        public void AddPatient(Patient patient)
        {
            var patients = ReadAllPatients();

            if (patients.Any(p => p.CI == patient.CI))
                throw new Exception("Patient with this CI already exists");

            using (var writer = File.AppendText(_config.PatientsFilePath))
            {
                writer.WriteLine($"{patient.Name},{patient.LastName},{patient.CI},{patient.BloodType}");
            }
        }

        // READ (Single)
        public Patient GetPatient(string ci)
        {
            using (var reader = new StreamReader(_config.PatientsFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');
                    if (parts[2] == ci)
                    {
                        return new Patient(parts[2], parts[0], parts[1])
                        {
                            BloodType = parts[3]
                        };
                    }
                }
            }
            throw new Exception("Patient not found");
        }

        // UPDATE
        public void UpdatePatient(string ci, Patient updatedPatient)
        {
            var patients = ReadAllPatients();
            var patient = patients.FirstOrDefault(p => p.CI == ci);

            if (patient == null)
                throw new Exception("Patient not found");

            patient.Name = updatedPatient.Name;
            patient.LastName = updatedPatient.LastName;
            SaveAllPatients(patients);
        }

        // DELETE
        public void DeletePatient(string ci)
        {
            var patients = ReadAllPatients();
            var patient = patients.FirstOrDefault(p => p.CI == ci);

            if (patient == null)
                throw new Exception("Patient not found");

            patients.Remove(patient);
            SaveAllPatients(patients);
        }

        // READ (All)
        public List<Patient> GetPatients()
        {
            return ReadAllPatients();
        }

    }
}
