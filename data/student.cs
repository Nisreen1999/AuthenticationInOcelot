using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationAutherization.data
{
    public class student
    {

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
