namespace Attendence_System.Models
{
    public class StudentLecture
    {
        public int StudentLectureId { get; set; }  // معرف السجل
        public int StudentId { get; set; }  // معرف الطالب
        public Student Student { get; set; }  // العلاقة مع جدول الطلاب
        public int LectureId { get; set; }  // معرف المحاضرة
        public Lecture Lecture { get; set; }  // العلاقة مع جدول المحاضرات
    }
}
