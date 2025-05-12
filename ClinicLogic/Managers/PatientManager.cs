using ClinicLogic.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Text;

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
        public string GetProject3ApiUrl() => _config.Project3ApiUrl;
        // Helper, lee el archivo entero y devuelve una lista de pacientes
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
                        BloodType = parts[3],
                        PatientID = parts.Length > 4 ? parts[4].Trim() : null
                    });
                    Log.Information($"Paciente leído: {parts[0]} {parts[1]} con CI {parts[2]}, tipo de sangre {parts[3]} y PatientID {parts[4]}.");

                }
            }
            Log.Information($"Pacientes leídos del archivo {_config.PatientsFilePath}.");
            return patients;
        }

        // Helpeer, guarda todos los pacientes de una lista de pacientes
        private void SaveAllPatients(List<Patient> patients)
        {
            using (var writer = new StreamWriter(_config.PatientsFilePath))
            {
                foreach (var patient in patients)
                {
                    writer.WriteLine($"{patient.Name},{patient.LastName},{patient.CI},{patient.BloodType}, {patient.PatientID}");
                    Log.Information($"Paciente guardado: {patient.Name} {patient.LastName} con CI {patient.CI}, tipo de sangre {patient.BloodType} y PatientID {patient.PatientID}.");
                }
            }
            Log.Information($"Pacientes guardados en el archivo {_config.PatientsFilePath}.");
        }

        public async Task<Patient> AddPatient(Patient patient)
        {
            var patients = ReadAllPatients();

            if (patients.Any(p => p.CI == patient.CI))
                throw new Exception("Patient with this CI already exists");

            patient= await GeneratePatientID(patient.Name, patient.LastName, patient.CI);

            using (var writer = File.AppendText(_config.PatientsFilePath))
            {
                writer.WriteLine($"{patient.Name},{patient.LastName},{patient.CI},{patient.BloodType},{patient.PatientID}");
            }
            Log.Information($"Paciente {patient.Name} {patient.LastName} con CI {patient.CI} agregado con éxito.");
            return patient;
        }


        public Patient GetPatient(string ci)
        {
            Log.Information($"Buscando paciente con CI {ci} en el archivo {_config.PatientsFilePath}");
            using (var reader = new StreamReader(_config.PatientsFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(',');
                    if (parts[2] == ci)
                    {
                        Log.Information($"Paciente encontrado: {parts[0]} {parts[1]} con CI {parts[2]}, tipo de sangre {parts[3]} y PatientID {parts[4]}.");
                        return new Patient(parts[2], parts[0], parts[1])
                        {
                            BloodType = parts[3],
                            PatientID = parts.Length > 4 ? parts[4].Trim() : null
                        };
                    }
                }
            }
            Log.Error($"No se encontró el paciente con CI {ci} en el archivo {_config.PatientsFilePath}");
            throw new Exception("Patient not found");
        }

        public void UpdatePatient(string ci, Patient updatedPatient)
        {
            var patients = ReadAllPatients();
            var patient = patients.FirstOrDefault(p => p.CI == ci);

            if (patient == null)
            {
                Log.Error($"No se encontró el paciente con CI {ci} para actualizar.");
                throw new Exception("Patient not found");
            }
              
            Log.Information($"Actualizando paciente {patient.Name} {patient.LastName} con CI {ci}");
            patient.Name = updatedPatient.Name;
            patient.LastName = updatedPatient.LastName;
            Log.Information($"Paciente actualizado: {patient.Name} {patient.LastName} con CI {ci}");
            SaveAllPatients(patients);
        }

        public void DeletePatient(string ci)
        {
            var patients = ReadAllPatients();
            var patient = patients.FirstOrDefault(p => p.CI == ci);

            if (patient == null)
            {
                Log.Error($"No se encontró el paciente con CI {ci} para eliminar.");
                throw new Exception("Patient not found");
            }
                
            Log.Information($"Eliminando paciente {patient.Name} {patient.LastName} con CI {ci}");
            patients.Remove(patient);
            SaveAllPatients(patients);
        }

        public List<Patient> GetPatients()
        {
            return ReadAllPatients();
        }

        public async Task<Patient> GeneratePatientID(string name, string lastName, string ci)
        {
            
            Log.Information($"Generando PatientID para {name} {lastName} con CI {ci}");
            try
            {

                var patient = new Patient(ci, name, lastName);
                string json = JsonConvert.SerializeObject(patient);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient
                {
                    BaseAddress = new Uri(_config.Project3ApiUrl) 
                };

                HttpResponseMessage response = await client.PostAsync("api/patientcode", content);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                string? patientCode = responseBody;

                if (patientCode == null)
                {
                    Log.Error("La respuesta del servidor es nula.");
                    throw new InvalidOperationException("La respuesta deserializada es nula.");
                }
                    

                patient.PatientID = patientCode;
                Log.Information($"PatientID generado: {patient.PatientID}");
                return patient;

             

            }
            catch (Exception ex)
            {
                Log.Error($"Error al generar PatientID: {ex.Message}");
                throw;
            }

        }
    }
}
