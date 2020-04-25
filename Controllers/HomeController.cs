using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using finalGSA.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace finalGSA.Controllers
{
    // Hello Daesung
    public class HomeController : Controller
    {
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        private int? UserSession
        {
            get { return HttpContext.Session.GetInt32("UserId"); }
            set { HttpContext.Session.SetInt32("UserId", (int)value); }
        }

        // VIEWS
        public IActionResult LandingPage()
        {
            return View();
        }

        [HttpGet("strive_together/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("strive_together/select-role")]
        public IActionResult SelectRole()
        {
            return View();
        }

        [HttpGet("strive_together/helper")]
        public IActionResult HelperRegView()
        {
            return View();
        }

        [HttpGet("strive_together/needofhelp")]
        public IActionResult HelpRegView()
        {
            return View();
        }

        [HttpGet("strive_together/dashboard")]
        public IActionResult Dashboard()
        {
            if (UserSession == null)
            {
                return RedirectToAction("LandingPage");
            }
            User CurrentUser = dbContext.Users.FirstOrDefault(a => a.UserId == UserSession);
            int MaxZip = CurrentUser.ZipCode + 80;
            int MinZip = CurrentUser.ZipCode + -80;
            List<Request> NearbyRequests = new List<Request>();
            for (int zip = MinZip; zip <= MaxZip; zip++)
            {
                List<Request> FoundRequests = dbContext.Requests.Include(s => s.Creator).Where(x => x.Creator.ZipCode == zip).OrderBy(i => i.Urgency).ToList();
                if (FoundRequests != null)
                {
                    foreach (var item in FoundRequests)
                    {
                        NearbyRequests.Add(item);
                    }
                }
            }
            ViewBag.CurrentUser = CurrentUser;
            // List<Request> NearbyRequests = dbContext.Requests.Include (a => a.Creator).Where (a => a.Creator.ZipCode == CurrentUser.ZipCode).OrderBy (a => a.Urgency).ToList ();
            return View(NearbyRequests);
        }

        [HttpGet("strive_together/view/{Uid}/details")]
        public IActionResult Detail(int Uid)
        {
            if (UserSession == null)
            {
                return RedirectToAction("LandingPage");
            }
            User DetailsFor = dbContext.Users.Include(a => a.RequestsCreated).FirstOrDefault(q => q.UserId == Uid);
            User CurrentUser = dbContext.Users.FirstOrDefault(q => q.UserId == UserSession);
            ViewBag.CurrentUser = CurrentUser;
            foreach (var req in DetailsFor.RequestsCreated)
            {
                if (req.IsCompleted == true)
                {
                    User CompletedBy = dbContext.Users.FirstOrDefault(l => l.UserId == req.PickedUpByID);
                    req.CompletedBy = CompletedBy.FirstName + " " + CompletedBy.LastName;
                }
            }
            dbContext.SaveChanges();
            return View(DetailsFor);
        }

        [HttpGet("strive_together/detils/{UserId}/helper")]
        public IActionResult HelperDetails(int UserId)
        {
            if (UserSession == null)
            {
                return RedirectToAction("LandingPage");
            }
            User DetailsFor = dbContext.Users.FirstOrDefault(q => q.UserId == UserId);
            List<Request> CompletedReqs = dbContext.Requests.Include(a => a.Creator).Where(q => q.PickedUpByID == DetailsFor.UserId).ToList();
            ViewBag.CurrentUser = dbContext.Users.FirstOrDefault(q => q.UserId == UserSession);
            if (CompletedReqs.Any())
            {
                ViewBag.Completed = CompletedReqs;
            }
            else
            {
                ViewBag.Completed = null;
            }
            return View(DetailsFor);
        }

        public IActionResult RequestForm()
        {
            if (UserSession == null)
            {
                return RedirectToAction("LandingPage");
            }
            return View();
        }

        [HttpPost("strive_together/register/help-user")]
        public IActionResult HelpReg(User HelpUser)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.Users.Any(i => i.Email == HelpUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists!");
                    return View("HelpRegView");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                string hashedPw = hasher.HashPassword(HelpUser, HelpUser.Password);
                HelpUser.Password = hashedPw;
                dbContext.Add(HelpUser);
                dbContext.SaveChanges();
                UserSession = HelpUser.UserId;
                return RedirectToAction("RequestForm");
            }
            return View("HelpRegView");
        }

        [HttpPost("strive_together/register/helper-user")]
        public IActionResult HelperReg(User HelperUser)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.Users.Any(i => i.Email == HelperUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists!");
                    return View("HelperRegView");
                }
                PasswordHasher<User> hasher = new PasswordHasher<User>();
                string hashedPw = hasher.HashPassword(HelperUser, HelperUser.Password);
                HelperUser.Password = hashedPw;
                dbContext.Add(HelperUser);
                dbContext.SaveChanges();
                UserSession = HelperUser.UserId;
                return RedirectToAction("Dashboard");
            }
            return View("HelperRegView");
        }

        [HttpPost("strive_together/login/user")]
        public IActionResult LoginUser(Login currentUser)
        {
            if (ModelState.IsValid)
            {
                var UserInDb = dbContext.Users.FirstOrDefault(i => i.Email == currentUser.LoginEmail);
                if (UserInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Credentials");
                    return View("Login");
                }
                var hasher = new PasswordHasher<Login>();
                var result = hasher.VerifyHashedPassword(currentUser, UserInDb.Password, currentUser.LoginPassword);
                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Credentials");
                    return View("Login");
                }
                UserSession = UserInDb.UserId;
                return RedirectToAction("Dashboard");
            }
            return View("Login");
        }

        [HttpPost("strive_together/request-help")]
        public IActionResult submitRequest(Request newRequest)
        {
            if (ModelState.IsValid)
            {
                User userfromDb = dbContext.Users.FirstOrDefault(a => a.UserId == UserSession);
                newRequest.UserID = userfromDb.UserId;
                dbContext.Add(newRequest);
                dbContext.SaveChanges();
                return RedirectToAction("Detail", new { Uid = userfromDb.UserId });
            }
            return View("RequestForm");
        }

        [HttpPost("strive_together/complete/request/{reqId}")]
        public IActionResult CompleteReq(int reqId)
        {
            if (ModelState.IsValid)
            {
                Request requestFromDb = dbContext.Requests.Include(a => a.Creator).FirstOrDefault(a => a.RequestId == reqId);
                requestFromDb.IsCompleted = true;
                requestFromDb.PickedUpByID = (int)UserSession;
                dbContext.SaveChanges();
                return RedirectToAction("Detail", new { Uid = requestFromDb.Creator.UserId });
            }
            return View("Details");
        }

        [HttpGet("delete")]
        public IActionResult Delete(int requestId)
        {
            if (UserSession == null)
                return RedirectToAction("Index");

            var RequestToDelete = dbContext.Requests.Include(i => i.Creator).FirstOrDefault(i => i.RequestId == requestId);
            dbContext.Requests.Remove(RequestToDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Detail", new { Uid = RequestToDelete.Creator.UserId });
        }

        [HttpPost("strive_together/cancel/request/{reqId}")]
        public IActionResult CancelReq(int reqId)
        {
            if (ModelState.IsValid)
            {
                Request CancelReq = dbContext.Requests.FirstOrDefault(a => a.RequestId == reqId);
                int x = CancelReq.PickedUpByID;
                CancelReq.IsCompleted = false;
                CancelReq.PickedUpByID = 0;
                dbContext.SaveChanges();
                return RedirectToAction("HelperDetails", new { UserId = x });
            }
            return View("Details");
        }

        // adding avatar
        [HttpPost("helper-avatar/{UserId}")]
        public IActionResult HelperAvatar(string imageurl, int UserId)
        {
            if (ModelState.IsValid)
            {
                User CurrentUser = dbContext.Users.FirstOrDefault(w => w.UserId == UserId);
                CurrentUser.ImageUrl = imageurl;
                dbContext.SaveChanges();
                return RedirectToAction("HelperDetails", new { UserId = UserId });
            }
            return View("HelperDetails");
        }
        [HttpPost("avatar/{UserId}")]
        public IActionResult NeedHelpAvatar(string imageurl, int UserId)
        {
            if (ModelState.IsValid)
            {
                User CurrentUser = dbContext.Users.FirstOrDefault(w => w.UserId == UserId);
                CurrentUser.ImageUrl = imageurl;
                dbContext.SaveChanges();
                return RedirectToAction("Detail", new { Uid = UserId });
            }
            return View("Detail");
        }

        [HttpGet("strive_together/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("LandingPage");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}