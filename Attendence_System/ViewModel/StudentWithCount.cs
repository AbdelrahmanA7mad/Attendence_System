namespace Attendence_System.ViewModel
{
    public class StudentWithCount
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string idcollege { get; set; }
        public string code { get; set; }
        public string SenderName { get; set; }
        public int Count { get; set; }  // عدد مرات التكرار
    }
}
