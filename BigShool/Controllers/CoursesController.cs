using BigShool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigShool.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        public ActionResult Create()
        {
            BigSchoolContext context = new BigSchoolContext();
            Course course = new Course();
            course.ListCategory = context.Categories.ToList();
            return View(course);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course cs)
        {
            BigSchoolContext context = new BigSchoolContext();
            ModelState.Remove("LectureId");
            if (!ModelState.IsValid)
            {
                cs.ListCategory = context.Categories.ToList();
                return View("Create", cs);
            }
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            cs.LectureId = user.Id;
            context.Courses.Add(cs);
            context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAtt = context.Attendances.Where(p => p.Attendee == user.Id).ToList();
            var course = new List<Course>();
            foreach (Attendance temp in listAtt)
            {
                Course course1 = temp.Course;
                course1.Name = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(course1.LectureId).Name;
                course.Add(course1);

            }
            return View(course);
        }

        public ActionResult Mine()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var course = context.Courses.Where(c => c.LectureId == user.Id && c.Dateime > DateTime.Now).ToList();
            foreach (Course i in course)
            {
                i.Name = user.Name;
            }
            return View(course);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Course cs = context.Courses.Where(p => p.Id == id).FirstOrDefault();
            if (cs == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(cs);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult Xacnhanxoa(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            Course cs = context.Courses.Where(n => n.Id == id).FirstOrDefault();
            context.Courses.Remove(cs);
            context.SaveChanges();
            return RedirectToAction("Mine");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Course cs = context.Courses.Where(p => p.Id == id).FirstOrDefault();
            cs.ListCategory = context.Categories.ToList();
            if (cs == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(cs);
        }
        [HttpPost, ActionName("Edit")]
        public ActionResult Xacnhansua(int id)
        {
            BigSchoolContext context = new BigSchoolContext();
            Course cs = context.Courses.FirstOrDefault(n => n.Id == id);
            UpdateModel(cs); ;
            context.SaveChanges();
            return RedirectToAction("Mine");
        }

        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser =
           System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
            .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();

            var listFollwee = context.Followings.Where(p => p.FollowerId == currentUser.Id).ToList();
            var listAttendances = context.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (var course in listAttendances)
            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.LectureId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LectureName =
                        System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                        .FindById(objCourse.LectureId).Name;
                        courses.Add(objCourse);
                    }
                }

            }
            return View(courses);
        }
    }
}