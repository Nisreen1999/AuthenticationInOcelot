using AuthenticationAutherization.data;

namespace AuthenticationAutherization.services
{
    public class StudentService : IStudentService
    {
        ContextClass context;
        public StudentService(ContextClass _context)
        {
            context = _context;
        }
        public List<student> StudentList()
        {
            return context.student.ToList();
        }
    }
}
