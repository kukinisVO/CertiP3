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
        public PatientController()
        {
            // Initialize the PatientManager here if needed
        }

        [HttpGet]
        [Route("{ci}")]
        public Patient GetPatient(int ci)
        {
            try
            {
                PatientManager patient = new PatientManager();
                var patientData = patient.GetPatient(ci);
                return patientData;
            }
            catch (Exception ex)
            {
                return new Patient(0,"","");
            }
        }

    }
}
