using System;
using System.Web.Mvc;
using SampleProject.DAL;
using SampleProject.Models;
using NLog;

namespace SampleProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        MasterDataDAL dal = new MasterDataDAL();
        AuthenticationDAL ADAL = new AuthenticationDAL();

        // =====================================================
        // ADMIN DASHBOARD
        // =====================================================
        public ActionResult Index()
        {
            try
            {
                ViewBag.Managers = dal.GetAllManagers();
                ViewBag.Clients = dal.GetAllClients();
                ViewBag.Projects = dal.GetAllProjects();
                ViewBag.ActivityTypes = dal.GetAllActivityTypes();
                ViewBag.Departments = ADAL.GetDepartments();

                logger.Info("Admin Index loaded");

                return View();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Admin Index failed");
                return View("Error");
            }
        }

        // =====================================================
        // ADD MANAGER
        // =====================================================
        [HttpPost]
        public ActionResult AddManager(string name, string email, string password, int departmentId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    TempData["Error"] = "Name required.";
                    return RedirectToAction("Index");
                }

                string emailError;
                if (!IsEmailValidAndAvailable(email, out emailError))
                {
                    TempData["Error"] = emailError;
                    return RedirectToAction("Index");
                }

                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    TempData["Error"] = "Password must be at least 6 characters.";
                    return RedirectToAction("Index");
                }

                if (departmentId <= 0)
                {
                    TempData["Error"] = "Select department.";
                    return RedirectToAction("Index");
                }

                ADAL.RegisterManger(name, email, password, null, departmentId);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "AddManager failed");
                return View("Error");
            }
        }

        // =====================================================
        // DELETE MANAGER
        // =====================================================
        [HttpPost]
        public ActionResult DeleteManager(int id)
        {
            try
            {
                if (id <= 0)
                    return RedirectToAction("Index");

                bool deleted = dal.DeleteManager(id);

                if (!deleted)
                {
                    TempData["Error"] = "Unable to delete manager.";
                    return RedirectToAction("Index");
                }

                logger.Info("DeleteManager: id={0}", id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "DeleteManager failed: id={0}", id);
                return View("Error");
            }
        }

        // =====================================================
        // ADD CLIENT
        // =====================================================
        [HttpPost]
        public ActionResult AddClient(string name, string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                {
                    TempData["Error"] = "Name and email are required.";
                    return RedirectToAction("Index");
                }

                bool inserted = dal.InsertClient(name, email);

                if (!inserted)
                {
                    TempData["Error"] = "Unable to add client.";
                    return RedirectToAction("Index");
                }

                logger.Info("AddClient: name={0} email={1}", name, email);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "AddClient failed: name={0} email={1}", name, email);
                return View("Error");
            }
        }

        // =====================================================
        // DELETE CLIENT
        // =====================================================
        [HttpPost]
        public ActionResult DeleteClient(int id)
        {
            try
            {
                if (id <= 0)
                    return RedirectToAction("Index");

                bool deleted = dal.DeleteClient(id);

                if (!deleted)
                {
                    TempData["Error"] = "Unable to delete client.";
                    return RedirectToAction("Index");
                }

                logger.Info("DeleteClient: id={0}", id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "DeleteClient failed: id={0}", id);
                return View("Error");
            }
        }

        // =====================================================
        // ADD PROJECT
        // =====================================================
        [HttpPost]
        public ActionResult AddProject(string projectName, int clientId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(projectName) || clientId <= 0)
                {
                    TempData["Error"] = "Project name and client are required.";
                    return RedirectToAction("Index");
                }

                bool inserted = dal.InsertProject(projectName, clientId);

                if (!inserted)
                {
                    TempData["Error"] = "Unable to add project.";
                    return RedirectToAction("Index");
                }

                logger.Info("AddProject: name={0} clientId={1}", projectName, clientId);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "AddProject failed: name={0} clientId={1}", projectName, clientId);
                return View("Error");
            }
        }

        // =====================================================
        // DELETE PROJECT
        // =====================================================
        [HttpPost]
        public ActionResult DeleteProject(int id)
        {
            try
            {
                if (id <= 0)
                    return RedirectToAction("Index");

                bool deleted = dal.DeleteProject(id);

                if (!deleted)
                {
                    TempData["Error"] = "Unable to delete project.";
                    return RedirectToAction("Index");
                }

                logger.Info("DeleteProject: id={0}", id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "DeleteProject failed: id={0}", id);
                return View("Error");
            }
        }

        // =====================================================
        // ADD ACTIVITY TYPE
        // =====================================================
        [HttpPost]
        public ActionResult AddActivityType(string activityName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(activityName))
                {
                    TempData["Error"] = "Activity name is required.";
                    return RedirectToAction("Index");
                }

                bool inserted = dal.InsertActivityType(activityName);

                if (!inserted)
                {
                    TempData["Error"] = "Unable to add activity type.";
                    return RedirectToAction("Index");
                }

                logger.Info("AddActivityType: name={0}", activityName);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "AddActivityType failed: name={0}", activityName);
                return View("Error");
            }
        }

        // =====================================================
        // DELETE ACTIVITY TYPE
        // =====================================================
        [HttpPost]
        public ActionResult DeleteActivityType(int id)
        {
            try
            {
                if (id <= 0)
                    return RedirectToAction("Index");

                bool deleted = dal.DeleteActivityType(id);

                if (!deleted)
                {
                    TempData["Error"] = "Unable to delete activity type.";
                    return RedirectToAction("Index");
                }

                logger.Info("DeleteActivityType: id={0}", id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "DeleteActivityType failed: id={0}", id);
                return View("Error");
            }
        }

        private bool IsEmailValidAndAvailable(string email, out string error)
        {
            error = null;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
            }
            catch
            {
                error = "Invalid email format.";
                return false;
            }

            if (ADAL.GetUserIdByEmail(email) > 0)
            {
                error = "Email already exists.";
                return false;
            }

            return true;
        }
    }
}