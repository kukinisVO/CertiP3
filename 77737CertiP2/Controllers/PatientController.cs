using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClinicLogic.Models;
using ClinicLogic.Managers;
using Serilog.Core;
using Serilog;

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

        [HttpPost]
        [Route("{ci}")]
        public Patient PostPatient([FromBody] Patient patient) // Directly use Patient model
        {
            try
            {
                _patientManager.AddPatient(patient);
                Log.Information("Patient added successfully: {@Patient}", patient);
                return patient;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error adding patient");
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
                Log.Error(ex, "Error retrieving patient");
                return new Patient("0", "NULL", "NULL");
            }
        }

        [HttpPut("{ci}")]
        public Patient PutPatient(string ci, [FromBody] Patient updatedPatient)
        {
            try
            {
                _patientManager.UpdatePatient(ci, updatedPatient);
                Log.Information("Patient updated successfully: {@Patient}", updatedPatient);
                return updatedPatient;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating patient");
                return new Patient("0", "NULL", "NULL");
            }
        }

        [HttpDelete("{ci}")]
        public Patient DeletePatient(string ci)
        {
            try
            {
                _patientManager.DeletePatient(ci);
                Log.Information("Patient deleted successfully: {CI}", ci);
                return new Patient("0", "NULL", "NULL");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting patient");
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
                Log.Error(ex, "Error retrieving patients");
                return new List<Patient>() { new Patient("0", "LISTA", "VACIA") };
            }
        }
    }
}
