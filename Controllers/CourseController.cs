using AuthenticationAutherization;
using AuthenticationAutherization.data;
using AuthenticationAutherization.services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Api.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
     [Authorize(Roles = "Student")]
  
    public class CourseController : ControllerBase
    {
        ICourseService _service;
        public CourseController(ICourseService service)
        {
            _service = service;
        }
        [HttpGet]
        [Route("ListOfCourse")]
        public List<Course> ListOfCourse()
        {
            return _service.CourseList();
        }
    }
}
