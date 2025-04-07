namespace Attendence_System.ViewModel
{
    public class LectureWithStudentCount
    {
        public int LectureId { get; set; }
        public string LectureTitle { get; set; }
        public List<StudentWithCount> Students { get; set; }
    }
}
