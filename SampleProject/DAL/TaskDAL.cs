using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using SampleProject.Models;

namespace SampleProject.DAL
{
    public class TaskDAL
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        //==========================================================
        //Get Id Of User
        //=========================================================
        public int GetUserIdByEmail(string email)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
             
                string query = "SELECT employee_id FROM employee WHERE email=@Email";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);

                con.Open();

                object result = cmd.ExecuteScalar();

                return result == null ? 0 : Convert.ToInt32(result);
            }
        }
        // ==============================
        // ADD TASK
        // ==============================
        public bool AddTask(string taskName, DateTime taskDate, int clientId, int projectId, int userId, int managerId)
        {
            try
            {
                
                using (SqlConnection con = new SqlConnection(connectionString))
                {

                    string query = "INSERT INTO task(task_name,task_date,task_client,task_project,task_status,user_id,manager_id) VALUES(@Name,@Date,@Client,@Project,'Pending',@User,@Manager)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Name", taskName); cmd.Parameters.AddWithValue("@Date", taskDate); cmd.Parameters.AddWithValue("@Client", clientId); cmd.Parameters.AddWithValue("@Project", projectId); cmd.Parameters.AddWithValue("@User", userId); cmd.Parameters.AddWithValue("@Manager", managerId);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }

        // ==============================
        // GET MANAGER ID
        // ==============================

        public int GetManagerId(int userId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query =
                    "SELECT manager_id FROM employee WHERE employee_id=@Id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", userId);

                con.Open();

                object result = cmd.ExecuteScalar();

                return result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
        }

        // ==============================
        // GET MANAGER NAME
        // ==============================

        public string GetManagerName(int userId)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT m.name
            FROM employee e
            JOIN employee m 
                ON e.manager_id = m.employee_id
            WHERE e.employee_id=@UserId AND m.is_active = 1";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@UserId", userId);

                con.Open();

                object result = cmd.ExecuteScalar();

                return result?.ToString();
            }
        }

        // ==============================
        // GET TASK BY ID
        // ==============================
        public TaskListModel GetTaskById(int taskId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT task_id,user_id FROM task WHERE task_id=@TaskId";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        return new TaskListModel { TaskId = Convert.ToInt32(dr["task_id"]), AssignedTo = Convert.ToInt32(dr["user_id"]) };
                    }
                }
            }
            catch { }
            return null;
        }


        // ==============================
        // GET TASKS BY USER
        // ==============================
        public List<TaskListModel> GetTasksByUser(int userId)
        {
            List<TaskListModel> list = new List<TaskListModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT t.task_id,t.task_name,t.task_date,t.task_status,c.client_name,p.project_name,ISNULL(SUM(a.hours),0) AS TotalHours FROM task t LEFT JOIN activity a ON a.task_id=t.task_id LEFT JOIN client c ON c.client_id=t.task_client LEFT JOIN project p ON p.project_id=t.task_project WHERE t.user_id=@UserId GROUP BY t.task_id,t.task_name,t.task_date,t.task_status,c.client_name,p.project_name ORDER BY t.task_date DESC, t.task_id DESC";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new TaskListModel
                        {
                            TaskId = Convert.ToInt32(dr["task_id"]),
                            Title = dr["task_name"].ToString(),
                            TaskDate = Convert.ToDateTime(dr["task_date"]),
                            Status = dr["task_status"].ToString(),
                            ClientName = dr["client_name"].ToString(),
                            ProjectName = dr["project_name"].ToString(),
                            TotalHours = Convert.ToDecimal(dr["TotalHours"])
                        });
                    }
                }
            }
            catch { }

            return list;
        }


        // ==============================
        // ADD ACTIVITY TO TASK
        // ==============================
        public bool AddActivity(int taskId, int userId, int activityTypeId, string title, string description, decimal hours)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO activity(task_id,user_id,activity_type_id,title,description,hours,activity_date) VALUES(@Task,@User,@Type,@Title,@Desc,@Hours,GETDATE())";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Task", taskId); cmd.Parameters.AddWithValue("@User", userId); cmd.Parameters.AddWithValue("@Type", activityTypeId); cmd.Parameters.AddWithValue("@Title", title); cmd.Parameters.AddWithValue("@Desc", description ?? ""); cmd.Parameters.AddWithValue("@Hours", hours);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch { return false; }
        }


        // ==============================
        // GET TASKS BY STATUS
        // ==============================
        public List<TaskListModel> GetTasksByStatus(int userId, string status)
        {
            List<TaskListModel> list = new List<TaskListModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT task_id,task_name,task_date,task_status FROM task WHERE user_id=@UserId AND task_status=@Status ORDER BY task_date DESC";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserId", userId); cmd.Parameters.AddWithValue("@Status", status);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new TaskListModel
                        {
                            TaskId = Convert.ToInt32(dr["task_id"]),
                            Title = dr["task_name"].ToString(),
                            TaskDate = Convert.ToDateTime(dr["task_date"]),
                            Status = dr["task_status"].ToString()
                        });
                    }
                }
            }
            catch { return null;  }

            return list;
        }


        // ==============================
        // GET ACTIVITY TYPES
        // ==============================
        public List<ActivityListModel> GetActivityTypes()
        {
            List<ActivityListModel> list = new List<ActivityListModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT activity_type_id,activity_type_name FROM activity_type WHERE is_active = 1";
                    SqlCommand cmd = new SqlCommand(query, con);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new ActivityListModel
                        {
                            ActivityTypeId = Convert.ToInt32(dr["activity_type_id"]),
                            ActivityTypeName = dr["activity_type_name"].ToString()
                        });
                    }
                }
            }
            catch { return null; }

            return list;
        }
        // ==============================
        // GET TASKS BY MANAGER
        // ==============================
        public List<TaskListModel> GetTasksByManager(int managerId)
        {
            List<TaskListModel> list = new List<TaskListModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT t.task_id,
                                            t.task_name,
                                            t.task_date,
                                            t.task_status,
                                            c.client_name,
                                            p.project_name,
                                            ISNULL(SUM(a.hours),0) AS TotalHours
                                        FROM task t
                                        INNER JOIN employee e ON e.employee_id = t.user_id
                                        LEFT JOIN activity a ON a.task_id = t.task_id
                                        LEFT JOIN client c ON c.client_id = t.task_client
                                        LEFT JOIN project p ON p.project_id = t.task_project
                                        WHERE e.manager_id = @ManagerId
                                        GROUP BY 
                                            t.task_id,t.task_name,t.task_date,
                                            t.task_status,c.client_name,p.project_name
                                        ORDER BY t.task_date DESC";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ManagerId", managerId);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new TaskListModel
                        {
                            TaskId = Convert.ToInt32(dr["task_id"]),
                            Title = dr["task_name"].ToString(),
                            TaskDate = Convert.ToDateTime(dr["task_date"]),
                            Status = dr["task_status"].ToString(),
                            ClientName = dr["client_name"].ToString(),
                            ProjectName = dr["project_name"].ToString(),
                            TotalHours = Convert.ToDecimal(dr["TotalHours"])
                        });
                    }
                }
            }
            catch { return null; }

            return list;
        }

        // ==============================
        // GET CLIENTS
        // ==============================
        public List<ClientModel> GetClients()
        {
            List<ClientModel> list = new List<ClientModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT client_id,client_name FROM client WHERE is_active = 1";
                    SqlCommand cmd = new SqlCommand(query, con);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new ClientModel
                        {
                            ClientId = Convert.ToInt32(dr["client_id"]),
                            ClientName = dr["client_name"].ToString()
                        });
                    }
                }
            }
            catch { return null; }

            return list;
        }
        // ==============================
        // MARK TASK AS COMPLETED
        // ==============================
        public bool CompleteTask(int taskId, int userId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query =
                        "UPDATE task SET task_status='Completed' WHERE task_id=@TaskId AND user_id=@UserId";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }
        // ==============================
        // GET ACTIVITIES BY TASK
        // ==============================
        public List<ActivityListModel> GetActivitiesByTask(int taskId, int userId)
        {
            List<ActivityListModel> list = new List<ActivityListModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT 
                    a.activity_id,
                    a.title,
                    a.description,
                    a.hours,
                    at.activity_type_name
                FROM activity a
                LEFT JOIN activity_type at 
                    ON at.activity_type_id = a.activity_type_id
                WHERE a.task_id = @TaskId
                AND a.user_id = @UserId
                ORDER BY a.activity_id";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    cmd.Parameters.AddWithValue("@UserId", userId);

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
                            ActivityTypeName = dr["activity_type_name"].ToString()
                        });
                    }
                }
            }
            catch { return null; }

            return list;
        }
        // ==============================
        // DELETE TASK
        // ==============================
        public bool DeleteTask(int taskId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                DELETE FROM activity WHERE task_id = @TaskId;
                DELETE FROM task WHERE task_id = @TaskId;";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }
        // ==============================
        // DELETE ACTIVITY
        // ==============================
        public bool DeleteActivity(int activityId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"
                DELETE FROM activity
                WHERE activity_id = @ActivityId";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ActivityId", activityId);
                    

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        // ==============================
        // GET PROJECTS BY CLIENT
        // ==============================
        public List<ProjectModel> GetProjectsByClient(int clientId)
        {
            List<ProjectModel> list = new List<ProjectModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT project_id,project_name FROM project WHERE client_id=@ClientId AND is_active = 1";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@ClientId", clientId);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new ProjectModel
                        {
                            ProjectId = Convert.ToInt32(dr["project_id"]),
                            ProjectName = dr["project_name"].ToString()
                        });
                    }
                }
            }
            catch { return null; }

            return list;
        }
    }
}