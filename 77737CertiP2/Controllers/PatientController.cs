using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClinicLogic.Models;

namespace _77737CertiP2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private static List<Patient> patients = new List<Patient>();

        // Crear paciente (HTTP POST)
        [HttpPost]
        public IActionResult CreatePatient([FromBody] Patient patient)
        {
            if (patients.Any(p => p.CI == patient.CI))
            {
                return BadRequest("A patient with the same CI already exists.");
            }

            patient.BloodType = patient.RandomBloodType();
            patients.Add(patient);
            return Ok($"Patient {patient.FirstName} {patient.LastName} with CI {patient.CI} has been added.");
        }

        // Actualizar paciente (HTTP PUT)
        [HttpPut("{ci}")]
        public IActionResult UpdatePatient(int ci, [FromBody] Patient updatedPatient)
        {
            var patient = patients.FirstOrDefault(p => p.CI == ci);
            if (patient == null)
            {
                return NotFound("Patient not found.");
            }

            patient.FirstName = updatedPatient.FirstName;
            patient.LastName = updatedPatient.LastName;
            return Ok($"Patient with CI {ci} has been updated.");
        }

        // Eliminar paciente (HTTP DELETE)
        [HttpDelete("{ci}")]
        public IActionResult DeletePatient(int ci)
        {
            var patient = patients.FirstOrDefault(p => p.CI == ci);
            if (patient == null)
            {
                return NotFound("Patient not found.");
            }

            patients.Remove(patient);
            return Ok($"Patient with CI {ci} has been deleted.");
        }

        // Obtener todos los pacientes (HTTP GET)
        [HttpGet]
        public IActionResult GetAllPatients()
        {
            return Ok(patients);
        }

        // Obtener paciente por CI (HTTP GET/{ci})
        [HttpGet("{ci}")]
        public IActionResult GetPatientByCI(int ci)
        {
            var patient = patients.FirstOrDefault(p => p.CI == ci);
            if (patient == null)
            {
                return NotFound("Patient not found.");
            }

            return Ok(patient);
        }
    }
}
