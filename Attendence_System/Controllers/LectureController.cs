using Attendence_System.Models;
using iTextSharp.text.pdf.qrcode;
using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System;
using Attendence_System.ViewModel;
using Attendence_System.Data;
using QRCoder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using iTextSharp.text;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Attendence_System.Controllers
{
    [Authorize]
    public class LectureController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public LectureController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // الحصول على الكورسات الخاصة بالمستخدم
            var userId = _userManager.GetUserId(User);
            var courses = await _db.Courses
                                   .Where(c => c.UserId == userId)  // عرض الكورسات الخاصة بالمستخدم
                                   .Select(c => new SelectListItem
                                   {
                                       Value = c.CourseId.ToString(),
                                       Text = c.Name
                                   })
                                   .ToListAsync();

            // إعداد النموذج وتمرير الكورسات الخاصة بالمستخدم
            var model = new LectureViewModel
            {
                Courses = courses  // استخدام الكورسات الخاصة بالمستخدم فقط
            };

            return View(model);
        }

        // POST: Add a new lecture
        [HttpPost]
        public async Task<IActionResult> Create(LectureViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // إعادة عرض النموذج مع الكورسات الخاصة بالمستخدم
                var user_Id = _userManager.GetUserId(User);
                model.Courses = await _db.Courses
                                          .Where(c => c.UserId == user_Id)
                                          .Select(c => new SelectListItem
                                          {
                                              Value = c.CourseId.ToString(),
                                              Text = c.Name
                                          })
                                          .ToListAsync();

                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            var lecture = new Lecture
            {
                Title = model.Title,
                CourseId = model.CourseId  // ربط المحاضرة بالكورس الذي تم اختياره
            };

            // إضافة المحاضرة أولاً ثم حفظها
            _db.Lectures.Add(lecture);
            await _db.SaveChangesAsync();  // حفظ المحاضرة وبالتالي تعيين LectureId

            // الآن يمكننا استخدام LectureId الصحيح لتوليد رمز الاستجابة السريعة
            var qrCodeBase64 = GenerateQRCode(lecture.LectureId);
            lecture.QRCode = qrCodeBase64;

            // حفظ التحديثات مع رمز الاستجابة السريعة
            _db.Lectures.Update(lecture);
            await _db.SaveChangesAsync();  // حفظ التغييرات التي تشمل QR code

            model.QRCode = qrCodeBase64;
            model.LectureId = lecture.LectureId;

            return View(model);
        }


        // Helper method to generate QR code
        private string GenerateQRCode(int lectureId)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(lectureId.ToString(), QRCodeGenerator.ECCLevel.Q);
            var qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
        }


        [HttpPost]
        public async Task<IActionResult> CloseAttendance(int id)
        {
            // العثور على المحاضرة في قاعدة البيانات
            var lecture = await _db.Lectures.FindAsync(id);

            if (lecture != null)
            {
                lecture.IsAttendanceClosed = true; // إغلاق المحاضرة لتسجيل الحضور
                _db.Lectures.Update(lecture);
                await _db.SaveChangesAsync(); // حفظ التحديثات في قاعدة البيانات
            }

            return RedirectToAction("Index", new { id }); // العودة إلى صفحة المحاضرات مع المعرف
        }
        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            // العثور على المحاضرة من قاعدة البيانات باستخدام id
            var lecture = await _db.Lectures
                         .Include(l => l.Course)  // تحميل الكورس المرتبط بالمحاضرة
                         .FirstOrDefaultAsync(l => l.LectureId == id);

            if (lecture == null)
            {
                return NotFound(); // إذا لم يتم العثور على المحاضرة
            }

            // الحصول على الـ UserId الخاص بالمستخدم الحالي
            var userId = _userManager.GetUserId(User);

            // التحقق إذا كان المستخدم هو صاحب المحاضرة
            if (lecture.Course.UserId != userId)
            {
                return Forbid(); // إذا لم يكن المستخدم هو صاحب المحاضرة
            }

            // العثور على الطلاب المرتبطين بهذه المحاضرة عبر جدول الربط StudentLecture
            var studentsInLecture = await _db.StudentLectures
                                              .Where(sl => sl.LectureId == id)
                                              .Include(sl => sl.Student)  // تضمين بيانات الطالب
                                              .Select(sl => sl.Student)
                                              .ToListAsync();


            

            // إنشاء ViewModel وتمرير البيانات إليه
            var model = new LectureWithStudentsViewModel
            {
                LectureTitle = lecture.Title,
                Students = studentsInLecture,
                StudentsCount = studentsInLecture.Count
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CalcAttendence(int id)
        {
            var course = await _db.Courses.FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound(); // إذا لم يتم العثور على الكورس
            }

            // جلب المحاضرات مع الطلاب المرتبطين بهم
            var lectures = await _db.Lectures
                .Include(l => l.StudentLectures)
                .ThenInclude(ls => ls.Student)
                .Where(d => d.CourseId == id)
                .ToListAsync();

            // تجميع الطلاب حسب idcollege وحساب عدد التكرارات لكل طالب
            var studentsWithCount = lectures
                .SelectMany(l => l.StudentLectures)
                .GroupBy(ls => ls.Student.idcollege)  // تجميع الطلاب حسب idcollege
                .Select(g => new StudentWithCount
                {
                    StudentId = g.First().Student.StudentId,
                    FullName = g.First().Student.FullName,
                    idcollege = g.Key,
                    code = g.First().Student.code,
                    SenderName = g.First().Student.SenderName,
                    Count = g.Count()  // حساب عدد التكرارات
                }).ToList();

            var viewModel = new CourseWithStudentsViewModel
            {
                CourseId = course.CourseId,
                CourseTitle = course.Name,
                StudentsWithCount = studentsWithCount  // ارسال قائمة الطلاب مع عدد التكرارات
            };

            return View(viewModel);
        }






        [HttpDelete]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _db.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound("Student not found");
            }
            _db.Students.Remove(student);
            await _db.SaveChangesAsync();
            return Ok("Student deleted successfully");
        }


    }
}