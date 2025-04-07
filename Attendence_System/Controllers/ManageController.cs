using Attendence_System.Data;
using Attendence_System.Models;
using Attendence_System.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Attendence_System.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ManageController> _logger;

        public ManageController(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<ManageController> logger)
        {
            _db = context;
            _userManager = userManager;
            _logger = logger;
        }

        // عرض صفحة إضافة الكورس والكورسات المضافة
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var userId = _userManager.GetUserId(User);
            var courses = await _db.Courses
                                   .Where(c => c.UserId == userId)
                                   .ToListAsync(cancellationToken);
            var model = new CourseViewModel
            {
                Courses = courses
            };
            return View(model);
        }

        // إضافة كورس جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CourseViewModel model)
        {

            var userId = _userManager.GetUserId(User);
            var newCourse = new Course
            {
                Name = model.Name,
                UserId = userId
            };

            _db.Courses.Add(newCourse);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // حذف كورس
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null)
            {
                return Json(new { success = false, message = "Course not found" });
            }

            var userId = _userManager.GetUserId(User);
            if (course.UserId != userId)
            {
                return Forbid();
            }

            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Course deleted successfully" });
        }

        // عرض المحاضرات المرتبطة بكورس
        [HttpGet]
        public async Task<IActionResult> ViewLectures(int id, int page = 1, int pageSize = 10)
        {
            // العثور على الكورس باستخدام AsNoTracking لتحسين الأداء
            var course = await _db.Courses
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound(); // إذا لم يتم العثور على الكورس
            }

            var userId = _userManager.GetUserId(User);

            // التحقق من ملكية المستخدم للكورس
            if (course.UserId != userId)
            {
                return Forbid(); // إذا لم يكن المستخدم هو صاحب الكورس
            }

            // استخدام AsNoTracking لتحسين الأداء
            var lectures = await _db.Lectures
                                    .AsNoTracking()
                                    .Where(l => l.CourseId == id)
                                    .Select(l => new { l.LectureId, l.Title }) // جلب الأعمدة الأساسية فقط
                                    .ToListAsync();


            var model = new LectureWithCourse
            {
                Lectures = lectures.Select(l => new Lecture { LectureId = l.LectureId, Title = l.Title }).ToList(),
                CouresName = course.Name,
                CourseId = course.CourseId, 
             
            };

            return View(model);
        }


        // حذف محاضرة
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLecture(int id)
        {
            var lecture = await _db.Lectures.FindAsync(id);
            if (lecture == null)
            {
                return Json(new { success = false, message = "Lecture not found" });
            }

            _db.Lectures.Remove(lecture);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Lecture deleted successfully" });
        }
    }
}
