using Attendence_System.Models;

namespace Attendence_System.ViewModel
{
    public class LectureWithStudentsViewModel
    {
        public int lectureid { get; set; }
        public int StudentsCount { get; set; }
        public string LectureTitle { get; set; }  // عنوان المحاضرة
        public List<Student> Students { get; set; }  // الطلاب المرتبطين بالمحاضرة
    }
}
