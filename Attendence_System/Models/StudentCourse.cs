using System.ComponentModel.DataAnnotations;

namespace Attendence_System.Models
{
    public class StudentCourse
    {
        [Key]
        public int StudentCouresId { get; set; }  // معرف السجل
        public int StudentId { get; set; }  // معرف الطالب
        public Student Student { get; set; }  // العلاقة مع جدول الطلاب
        public int CouresId { get; set; }  // معرف المحاضرة
        public Course Course { get; set; }  // العلاقة مع جدول المحاضرات
    }
}
