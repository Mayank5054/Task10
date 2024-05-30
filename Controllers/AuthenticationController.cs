using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task10.Models;

namespace Task10.Controllers
{
    public class AuthenticationController : Controller
    {
        ExamEntities _db;
        public AuthenticationController()
        {
            _db = new ExamEntities();
        }
        // GET: Authentication
        public ActionResult Login()
        {
            if (Session["UserId"] != null)
            {
                return RedirectToAction("AllProducts", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel loginModel)
        {
           
            if (ModelState.IsValid)
            {
                User _user = _db.Users.Where(x => (x.Email == loginModel.Email)).FirstOrDefault();
                if (_user != null && _user.Password == loginModel.password)
                {
                    Session["UserId"] = _user.UserId;
                    Session["ContactNo"] = _user.ContactNo;
                    Session["Type"] = _user.Type;
                    Session["Email"] = _user.Email;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.PasswordInvalid = "Email Or Password is invalid";
                    return View();
                }
            }
            else
            {
                return View();
            }
         
          
        }

        public ActionResult RegisterUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegisterUser(User _user)
        {
            User user = _db.Users.Where(x => x.Email == _user.Email).FirstOrDefault();
            if(user == null )
            {
                if (ModelState.IsValid)
                {
                    _db.Users.Add(_user);
                    _user.CreatedOn = DateTime.Now;
                    _user.UpdatedOn = DateTime.Now;
                    _user.Isdeleted = 0;
                    _db.SaveChanges();
                    TempData["Registered"] = "User Registered Succesffuly";
                    return RedirectToAction("Login", "Authentication");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                ViewBag.UserExists = "User Already Exists";
                    return View();
            }
      
        }

        public ActionResult logout()
        {
            Session["UserId"] = null;
            Session["ContactNo"] = null;
            Session["UserType"] = null;
            return RedirectToAction("Login", "Authentication");
        }

        public ActionResult UnAuthorized()
        {
            return View();
        }
    }
}