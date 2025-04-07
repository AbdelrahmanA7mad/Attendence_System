using Attendence_System.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Attendence_System.ViewModel
{
    public class CourseViewModel
    {
        public string Name { get; set; }
        public List<Course> Courses { get; set; }

    }
}
