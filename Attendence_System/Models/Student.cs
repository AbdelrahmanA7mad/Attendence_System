using Microsoft.EntityFrameworkCore;

namespace Attendence_System.Models
{
    public class Student
    {
        public int StudentId { get; set; }  // معرف الطالب
        public string FullName { get; set; }  // الاسم الكامل
        public string idcollege { get; set; }  // البريد الإلكتروني
        public string code { get; set; }  // رقم الهاتف

        public string ? AndroidId { get; set; }
        public string ? SenderName { get; set; }
        public int AttendenceNumber { get; set; }
        public ICollection<StudentLecture> StudentLectures { get; set; }
        public ICollection<StudentCourse> StudentCourse { get; set; }


    }
}
