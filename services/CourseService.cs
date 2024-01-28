using AuthenticationAutherization.data;

namespace AuthenticationAutherization.services
{
    public class CourseService : ICourseService
    {
        ContextClass context;
        public CourseService(ContextClass _context)
        {
            context = _context;
        }
        public List<Course> CourseList()
        {
            return context.course.ToList();
        }
    }
}
