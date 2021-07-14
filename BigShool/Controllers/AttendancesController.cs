using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BigShool.Models;
using Microsoft.AspNet.Identity;

namespace BigShool.Controllers
{
    public class AttendancesController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Attend(Course course)
        {
            var userID = User.Identity.GetUserId();
            BigSchoolContext context = new BigSchoolContext();
            if(context.Attendances.Any(p=>p.Attendee== userID  && p.CourseId == course.Id))
            {
                return BadRequest("The attendance already exists !");
            }
            var attendance = new Attendance() { CourseId = course.Id, Attendee = User.Identity.GetUserId() };
            context.Attendances.Add(attendance);
            context.SaveChanges();
            return Ok();
        }
    }
}
