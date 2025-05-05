using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClinicLogic.Models;
using ClinicLogic.Managers;

namespace _77737CertiP2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly PatientManager _patientManager;

        public PatientController(PatientManager patienntManager)
        {
            _patientManager = patienntManager;
        }

        [HttpGet("config-diagnostics")]
        public IActionResult CheckConfig()
        {
            var fullPath = Path.GetFullPath(_patientManager.GetFilePath()); // Ensure System.IO is used explicitly
            return Ok(new
            {
                ConfigValue = _patientManager.GetFilePath(),
                AbsolutePath = fullPath,
                FileExists = System.IO.File.Exists(fullPath), // Ensure System.IO is used explicitly
                Directory = Path.GetDirectoryName(fullPath) // Ensure System.IO is used explicitly
            });
        }

        [HttpPost]
        [Route("{ci}")]
        public Patient PostPatient([FromBody] Patient patient) // Directly use Patient model
        {
            try
            {
                _patientManager.AddPatient(patient);
                return patient;
            }
            catch (Exception ex)
            {
                return new Patient("0", "NULL", "NULL");
            }
        }

        [HttpGet("{ci}")]
        public Patient GetPatient(string ci)
        {
            try
            {
                var patient = _patientManager.GetPatient(ci);
                return patient;
            }
            catch (Exception ex)
            {
                return new Patient("0", "NULL", "NULL");
            }
        }

        [HttpPut("{ci}")]
        public Patient PutPatient(string ci, [FromBody] Patient updatedPatient)
        {
            try
            {
                _patientManager.UpdatePatient(ci, updatedPatient);
                return updatedPatient;
            }
            catch (Exception ex)
            {
                return new Patient("0", "NULL", "NULL");
            }
        }

        [HttpDelete("{ci}")]
        public Patient DeletePatient(string ci)
        {
            try
            {
                _patientManager.DeletePatient(ci);
                return new Patient("0", "NULL", "NULL");
            }
            catch (Exception ex)
            {
                return new Patient("0", "NULL", "NULL");
            }
        }

        [HttpGet]
        public List<Patient> GetPatients()
        {
            try
            {
                var patients = _patientManager.GetPatients();
                return patients;
            }
            catch (Exception ex)
            {
                return new List<Patient>() { new Patient("0", "LISTA", "VACIA") };
            }
        }
    }
}
