using System;
using System.Web.Mvc;
using SampleProject.DAL;
using SampleProject.Models;
using NLog;

public class EditController : Controller
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    AuthenticationDAL dal = new AuthenticationDAL();

    // ==============================
    // LOAD EDIT PAGE
    // ==============================
    public ActionResult EditDetails()
    {
        try
        {
            int userId = (int)Session["UserId"];
            Employee user = dal.GetUserProfile(userId);

            logger.Info("EditDetails loaded: userId={0}", userId);

            return View(user);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "EditDetails failed");
            return View("Error");
        }
    }

    // ==============================
    // SAVE CHANGES
    // ==============================
    [HttpPost]
    public ActionResult UpdateDetails(string UserName, string Email, string Password)
    {
        try
        {
            int userId = (int)Session["UserId"];
            bool result = dal.UpdateUserProfile(userId, UserName, Email, Password);

            logger.Info("UpdateDetails: userId={0} success={1}", userId, result);

            return Json(new { success = result });
        }
        catch (Exception ex)
        {
            logger.Error(ex, "UpdateDetails failed");
            return Json(new { success = false });
        }
    }
}