using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using SampleProject.Models;
using SampleProject.UserService;
using System.Data;
namespace SampleProject.DAL
{
    public class ManagerDAL
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        // ==============================
        // GET EMPLOYEES UNDER MANAGER
        // ==============================
        public List<EmployeeListModel> GetEmployeesUnderManager(int managerId)
        {
            List<EmployeeListModel> list = new List<EmployeeListModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT employee_id,name,email FROM employee WHERE manager_id=@ManagerId";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ManagerId", managerId);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new EmployeeListModel
                        {
                            UserId = Convert.ToInt32(dr["employee_id"]),
                            UserName = dr["name"].ToString(),
                            Email = dr["email"].ToString()
                        });
                    }
                }
            }
            catch { return null;  }

            return list;
        }

       
        // ==============================
        // GET ACTIVITIES BY TASK
        // ==============================
        public List<ActivityListModel> GetActivitiesByTaskForManager(int taskId)
        {
            List<ActivityListModel> list = new List<ActivityListModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT activity_id,title,description,hours,activity_date FROM activity WHERE task_id=@TaskId ORDER BY activity_date DESC";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@TaskId", taskId);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new ActivityListModel
                        {
                            ActivityId = Convert.ToInt32(dr["activity_id"]),
                            Title = dr["title"].ToString(),
                            Description = dr["description"].ToString(),
                            ActivityHours = Convert.ToDecimal(dr["hours"]),
                            ActivityDate = Convert.ToDateTime(dr["activity_date"])
                        });
                    }
                }
            }
            catch { return null; }

            return list;
        }


        // ==============================
        // GET PERFORMANCE HISTORY
        // ==============================
        public List<PerformanceModelInMVC> GetPerformanceHistory(int userId)
        {
            List<PerformanceModelInMVC> list = new List<PerformanceModelInMVC>();

            try
            {
                Service1Client client = new Service1Client();
                var results = client.GetPerformanceHistory(userId);

                foreach (var r in results)
                {
                    list.Add(new PerformanceModelInMVC
                    {
                        UserId = r.UserId,
                        AverageScore = r.AverageScore,
                        Feedback = r.Feedback
                    });
                }

                client.Close();
            }
            catch { return null; }

            return list;
        }
        public List<DepartmentPerformanceModel> GetDepartmentPerformance(int managerId)
        {
            List<DepartmentPerformanceModel> list =
                new List<DepartmentPerformanceModel>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd =
                    new SqlCommand("sp_GetDepartmentPerformance", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ManagerId", managerId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new DepartmentPerformanceModel
                    {
                        DepartmentId = Convert.ToInt32(dr["department_id"]),
                        DepartmentName = dr["department_name"].ToString(),
                        TotalEmployees = Convert.ToInt32(dr["TotalEmployees"]),
                        TotalReviews = Convert.ToInt32(dr["TotalReviews"]),
                        DepartmentAverageScore =
                            Convert.ToDecimal(dr["DepartmentAverageScore"])
                    });
                }
            }

            return list;
        }

        // ==============================
        // RATE EMPLOYEE
        // ==============================
        public bool RateEmployee(PerformanceModel model, int managerId)
        {
            try
            {
                Service1Client client = new Service1Client();

                var results = new UserService.PerformanceModel
                {
                    UserId = model.UserId,
                    TaskCompletion = model.TaskCompletion,
                    Productivity = model.Productivity,
                    Consistency = model.Consistency,
                    QualityOfWork = model.QualityOfWork,
                    Communication = model.Communication,
                    Teamwork = model.Teamwork,
                    Punctuality = model.Punctuality,
                    ProblemSolving = model.ProblemSolving,
                    Initiative = model.Initiative,
                    LearningAbility = model.LearningAbility,
                    Feedback = model.Feedback ?? ""
                };

                bool result = client.RateEmployee(results, managerId);
                client.Close();
                return result;
            }
            catch (Exception ex)
            {
 
                
                return false;
            }
        }
    }
}