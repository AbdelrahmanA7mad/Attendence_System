using NuGet.DependencyResolver;

namespace Attendence_System.Models
{
    public class Lecture
    {
        public int LectureId { get; set; }  // معرف المحاضرة
        public string Title { get; set; }  // عنوان المحاضرة
        public DateTime? DateTime { get; set; }  // تاريخ ووقت المحاضرة
        public string? QRCode { get; set; } // لتخزين المسار الخاص بالباركود
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public ICollection<StudentLecture> StudentLectures { get; set; }
        public bool IsAttendanceClosed { get; set; } // جديد

    }
}
