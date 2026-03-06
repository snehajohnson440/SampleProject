using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SampleProject.DAL;
using SampleProject.Models;
using NLog;

namespace SampleProject.Controllers
{
    [Authorize(Roles = "Employee,Manager")]
    public class TaskController : Controller
    {
        
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        TaskDAL dal = new TaskDAL();
        AuthenticationDAL authDal = new AuthenticationDAL();

        // ==============================
        // GET LOGGED USER ID
        // ==============================
        private int GetUserId()
        {
            string email = User.Identity.Name;
            return authDal.GetUserIdByEmail(email);
        }

        // ==============================
        // DELETE TASK
        // ==============================
        [HttpPost]
        public JsonResult DeleteTask(int id)
        {
            try
            {
                TaskDAL dal = new TaskDAL();
                bool success = dal.DeleteTask(id);

                logger.Info("DeleteTask: taskId={0} success={1}", id, success);

                return Json(new { success = success, taskId = id });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "DeleteTask failed: taskId={0}", id);
                return Json(new { success = false });
            }
        }

        // ==============================
        // TASK LIST PAGE
        // ==============================
        public ActionResult Index()
        {
            try
            {
                int userId = GetUserId();
                List<TaskListModel> tasks = dal.GetTasksByUser(userId);
                string managerName = dal.GetManagerName(userId);
                ViewBag.ManagerName = string.IsNullOrEmpty(managerName) ? "NO MANAGER NAME" : managerName;

                logger.Info("Index loaded: userId={0}", userId);

                return View(tasks);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Index failed");
                return View(new List<TaskListModel>());
            }
        }

        // ==============================
        // COMPLETE TASK
        // ==============================
        [HttpPost]
        public JsonResult CompleteTask(int taskId)
        {
            try
            {
                int userId = GetUserId();
                bool result = dal.CompleteTask(taskId, userId);

                logger.Info("CompleteTask: taskId={0} userId={1} success={2}", taskId, userId, result);

                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "CompleteTask failed: taskId={0}", taskId);
                return Json(new { success = false });
            }
        }

        // ==============================
        // ADD TASK
        // ==============================
        [HttpPost]
        public JsonResult AddTask(string taskName,
                                  DateTime taskDate,
                                  int clientId,
                                  int projectId)
        {
            try
            {
                int userId = GetUserId();
                int managerId = dal.GetManagerId(userId);

                // manager has no manager  assign self
                if (managerId == 0)
                    managerId = userId;

                bool result = dal.AddTask(taskName, taskDate, clientId, projectId, userId, managerId);

                logger.Info("AddTask: userId={0} taskName={1} date={2} success={3}", userId, taskName, taskDate, result);

                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "AddTask failed: taskName={0}", taskName);
                return Json(new { success = false });
            }
        }

        // ==============================
        // GET MANAGER NAME
        // ==============================
        [HttpGet]
        public JsonResult GetManagerName()
        {
            try
            {
                int userId = GetUserId();
                string managerName = dal.GetManagerName(userId);

                return Json(new { name = managerName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetManagerName failed");
                return Json(new { name = "" }, JsonRequestBehavior.AllowGet);
            }
        }

        // ==============================
        // GET TASKS BY STATUS
        // ==============================
        public JsonResult GetTasksByStatus(string status)
        {
            try
            {
                int userId = GetUserId();
                var tasks = dal.GetTasksByStatus(userId, status);

                return Json(tasks, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetTasksByStatus failed: status={0}", status);
                return Json(new List<TaskListModel>(), JsonRequestBehavior.AllowGet);
            }
        }

        // ==============================
        // ADD ACTIVITY
        // ==============================
        [HttpPost]
        public JsonResult AddActivity(AddActivityModel model)
        {
            try
            {
                if (model == null)
                {
                    logger.Warn("AddActivity: model is null");
                    return Json(new { success = false });
                }

                int userId = GetUserId();

                bool result = dal.AddActivity(
                    model.TaskId,
                    userId,
                    model.ActivityTypeId,
                    model.Title,
                    model.Description,
                    model.Hours);

                logger.Info("AddActivity: userId={0} taskId={1} success={2}", userId, model.TaskId, result);

                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "AddActivity failed");
                return Json(new { success = false });
            }
        }

        // ==============================
        // GET TASKS (DASHBOARD)
        // ==============================
        public JsonResult GetTasks(string date)
        {
            try
            {
                int userId = GetUserId();
                List<TaskListModel> tasks = dal.GetTasksByUser(userId);

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
        // GET ACTIVITIES
        // ==============================
        public JsonResult GetActivities(int taskId)
        {
            try
            {
                int userId = GetUserId();
                var activities = dal.GetActivitiesByTask(taskId, userId);

                return Json(activities, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetActivities failed: taskId={0}", taskId);
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        // ==============================
        // GET CLIENTS
        // ==============================
        public JsonResult GetClients()
        {
            try
            {
                return Json(dal.GetClients(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetClients failed");
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        // ==============================
        // DELETE ACTIVITY
        // ==============================
        [HttpPost]
        public JsonResult DeleteActivity(int id)
        {
            try
            {
                bool success = dal.DeleteActivity(id);

                logger.Info("DeleteActivity: activityId={0} success={1}", id, success);

                return Json(new { success = success });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "DeleteActivity failed: activityId={0}", id);
                return Json(new { success = false });
            }
        }

        // ==============================
        // GET PROJECTS BY CLIENT
        // ==============================
        public JsonResult GetProjects(int clientId)
        {
            try
            {
                return Json(dal.GetProjectsByClient(clientId), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetProjects failed: clientId={0}", clientId);
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        // ==============================
        // GET ACTIVITY TYPES
        // ==============================
        public JsonResult GetActivityTypes()
        {
            try
            {
                return Json(dal.GetActivityTypes(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "GetActivityTypes failed");
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }
    }
}