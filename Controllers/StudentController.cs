using AuthenticationAutherization;
using AuthenticationAutherization.data;
using AuthenticationAutherization.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teacher")]
    public class StudentController : ControllerBase
    {
        IStudentService _service;
        public StudentController(IStudentService service)
        {
            _service = service;
        }
     
        [HttpGet]
        [Route("ListOfStudent")]
        public List<student> ListOfStudent()
        {
            return _service.StudentList();
        }

    }
}
