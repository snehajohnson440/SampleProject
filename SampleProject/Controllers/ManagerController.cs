using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SampleProject.DAL;
using SampleProject.Models;
using SampleProject.UserService;
using NLog;

namespace SampleProject.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        TaskDAL taskDAL = new TaskDAL();
        ManagerDAL managerDAL = new ManagerDAL();

        // ==============================
        // GET USER ID
        // ==============================
        private int GetUserId()
        {
            string email = User.Identity.Name;
            return taskDAL.GetUserIdByEmail(email);
        }
        public JsonResult GetDepartmentPerformance()
        {
            int managerId = GetUserId();

            var data = managerDAL.GetDepartmentPerformance(managerId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        // ==============================
        // INDEX
        // ==============================
        public ActionResult Index()
        {
            try
            {
                int userId = GetUserId();
                ViewBag.UserId = userId;
                ViewBag.ManagerName = "NIL";

                logger.Info("Manager Index: userId={0}", userId);

                return View();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Manager Index failed");
                return View();
            }
        }

        // ==============================
        // PERFORMANCE REVIEW PAGE
        // ==============================
        public ActionResult PerformanceReview()
        {
            try
            {
                int userId = GetUserId();
                var employees = managerDAL.GetEmployeesUnderManager(userId);

                logger.Info("PerformanceReview: managerId={0} employeeCount={1}", userId, employees.Count);

                return View(employees);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "PerformanceReview failed");
                return View(new List<EmployeeListModel>());
            }
        }

        // ==============================
        // GET TASKS OF EMPLOYEE
        // ==============================
        [HttpGet]
        public JsonResult GetTasksOfEmployee(int employeeId)
        {
            try
            {
                var tasks = taskDAL.GetTasksByUser(employeeId);

                logger.Info("GetTasksOfEmployee: employeeId={0} count={1}", employeeId, tasks.Count);

                return Json(tasks, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetTasksOfEmployee failed: employeeId={0}", employeeId);
                return Json(new List<TaskListModel>(), JsonRequestBehavior.AllowGet);
            }
        }

        // ==============================
        // GET ACTIVITIES FOR MANAGER
        // ==============================
        [HttpGet]
        public JsonResult GetActivitiesForManager(int taskId)
        {
            try
            {
                var activities = managerDAL.GetActivitiesByTaskForManager(taskId);

                logger.Info("GetActivitiesForManager: taskId={0} count={1}", taskId, activities.Count);

                return Json(activities, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetActivitiesForManager failed: taskId={0}", taskId);
                return Json(new List<ActivityListModel>(), JsonRequestBehavior.AllowGet);
            }
        }

        // ==============================
        // GET TASKS (MANAGER DASHBOARD)
        // ==============================
        public JsonResult GetTasks(string date)
        {
            try
            {
                int userId = GetUserId();
                var tasks = taskDAL.GetTasksByUser(userId);

                if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime d))
                {
                    tasks = tasks.Where(t => t.TaskDate.Date == d.Date).ToList();
                }

                return Json(tasks, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetTasks failed: date={0}", date);
                return Json(new List<TaskListModel>(), JsonRequestBehavior.AllowGet);
            }
        }

        // ==============================
        // RATE EMPLOYEE
        // ==============================
        [HttpPost]
        public JsonResult RateEmployee(PerformanceModel model)
        {
            try
            {
                if (model == null)
                {
                    logger.Warn("RateEmployee: model is null");
                    return Json(new { success = false, message = "Model is null" });
                }

                if (model.UserId == 0)
                {
                    logger.Warn("RateEmployee: UserId is 0 - binding failed");
                    return Json(new { success = false, message = "UserId is 0 - binding failed" });
                }

                int managerId = GetUserId();
                var myEmployees = managerDAL.GetEmployeesUnderManager(managerId);

                if (!myEmployees.Any(e => e.UserId == model.UserId))
                {
                    logger.Warn("RateEmployee: unauthorized - managerId={0} targetUserId={1}", managerId, model.UserId);
                    return Json(new { success = false, message = "Unauthorized" });
                }

                bool result = managerDAL.RateEmployee(model, managerId);

                logger.Info("RateEmployee: managerId={0} employeeId={1} success={2}", managerId, model.UserId, result);

                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "RateEmployee failed: userId={0}", model?.UserId);
                return Json(new { success = false, message = "Server error" });
            }
        }

        // ==============================
        // GET PERFORMANCE HISTORY
        // ==============================
        [HttpGet]
        public JsonResult GetPerformanceHistory(int userId)
        {
            try
            {
                var data = managerDAL.GetPerformanceHistory(userId);

                logger.Info("GetPerformanceHistory: userId={0} count={1}", userId, data.Count);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetPerformanceHistory failed: userId={0}", userId);
                return Json(new List<PerformanceModelInMVC>(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}