using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SampleProject.Models;
using SampleProject.DAL;
using SampleProject.Security;
using NLog;

namespace SampleProject.Controllers
{
    public class AccountController : Controller
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        AuthenticationDAL authDal = new AuthenticationDAL();

        // ==============================
        // LOGIN GET
        // ==============================
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // ==============================
        // LOGIN POST
        // ==============================
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            try
            {//function call
                if (string.IsNullOrWhiteSpace(model.Email) ||
                    string.IsNullOrWhiteSpace(model.Password))
                {
                    ViewBag.Error = "Email and Password are required";
                    return View(model);
                }

                var user = authDal.Login(model.Email, model.Password);//call dal

                if (user == null)
                {
                    logger.Warn("Login failed: email={0}", model.Email);
                    ViewBag.Error = "Invalid email or password";
                    return View(model);
                }

                string token = JwtHelper.GenerateToken(user.UserId, user.Email, user.Role);
                //check token
                Response.Cookies.Add(new HttpCookie("jwt", token)
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddHours(1)
                });

                Session["UserId"] = user.UserId;
                Session["UserName"] = user.UserName;

                logger.Info("Login success: userId={0} email={1} role={2}", user.UserId, user.Email, user.Role);

                if (user.Role == "Manager")//check redirection
                    return RedirectToAction("Index", "Manager");

                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Admin");

                return RedirectToAction("Index", "Task");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Login exception: email={0}", model.Email);
                ViewBag.Error = "Something went wrong. Please try again.";
                return View(model);
            }
        }

        // ==============================
        // REGISTER GET
        // ==============================
        [AllowAnonymous]
        public ActionResult Register()
        {
            try
            {
               
                LoadRegisterDropdowns();
                return View();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Register GET failed");
                return View();
            }
        }

        // ==============================
        // REGISTER POST
        // ==============================
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(UserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadRegisterDropdowns();
                    return View(model);
                }

                if (authDal.GetUserIdByEmail(model.Email) > 0)
                {
                    logger.Warn("Register: email already exists: {0}", model.Email);
                    ViewBag.Error = "Email already registered";
                    LoadRegisterDropdowns();
                    return View(model);
                }

                bool success = authDal.Register(
                    model.UserName,
                    model.Email,
                    model.Password,
                    model.ManagerId,
                    model.DepartmentId
                );

                if (!success)
                {
                    logger.Warn("Register: DAL returned false for email={0}", model.Email);
                    ViewBag.Error = "Registration failed";
                    LoadRegisterDropdowns();
                    return View(model);
                }

                logger.Info("Register success: email={0}", model.Email);
                TempData["Success"] = "Account created successfully!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Register POST exception: email={0}", model.Email);
                ViewBag.Error = "Something went wrong.";
                LoadRegisterDropdowns();
                return View(model);
            }
        }

        // ==============================
        // LOGOUT
        // ==============================
        public ActionResult Logout()
        {
            try
            {
                if (Request.Cookies["jwt"] != null)
                {
                    var cookie = new HttpCookie("jwt") { Expires = DateTime.Now.AddDays(-1) };
                    Response.Cookies.Add(cookie);
                }

                string userName = Session["UserName"]?.ToString();
                Session.Clear();
                Session.Abandon();

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();

                logger.Info("Logout: user={0}", userName);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Logout exception");
            }

            return RedirectToAction("Login", "Account");
        }

        // ==============================
        // LOAD DROPDOWNS HELPER
        // ==============================
        private void LoadRegisterDropdowns()
        {
            var managers = authDal.GetManagers();

            if (managers == null)
                throw new Exception("Managers list is NULL");

            ViewBag.Managers =
                new SelectList(managers, "UserId", "UserName");

            var departments = authDal.GetDepartments();

            if (departments == null)
                throw new Exception("Departments list is NULL");

            ViewBag.Departments =
                new SelectList(departments, "DepartmentId", "DepartmentName");
        }
    }
}