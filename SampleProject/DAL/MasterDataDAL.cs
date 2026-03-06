using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SampleProject.Models;

namespace SampleProject.DAL
{
    public class MasterDataDAL
    {
        // Connection string from Web.config
        private string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // =====================================================
        // GET ALL MANAGERS
        // =====================================================
        public List<Employee> GetAllManagers()
        {
            List<Employee> managers = new List<Employee>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query =
                       "SELECT employee_id, name, email FROM employee WHERE role = 'Manager' AND is_active = 1";

                    SqlCommand cmd = new SqlCommand(query, con);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        managers.Add(new Employee
                        {
                            UserId = Convert.ToInt32(dr["employee_id"]),
                            UserName = dr["name"].ToString(),
                            Email = dr["email"].ToString()
                        });
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return managers;
        }
        // =====================================================
        // DELETE MANAGER
        // =====================================================
        public bool DeleteManager(int managerId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE employee SET is_active = 0 WHERE employee_id = @Id AND role = 'Manager'";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Id", managerId);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
        }

        // =====================================================
        // GET ALL CLIENTS
        // =====================================================
        public List<ClientModel> GetAllClients()
        {
            List<ClientModel> clients = new List<ClientModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM client WHERE is_active = 1";

                    SqlCommand cmd = new SqlCommand(query, con);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        clients.Add(new ClientModel
                        {
                            ClientId = Convert.ToInt32(dr["client_id"]),
                            ClientName = dr["client_name"].ToString(),
                            ClientEmail = dr["client_email"].ToString()
                        });
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return clients;
        }

        // =====================================================
        // INSERT CLIENT
        // =====================================================
        public bool InsertClient(string name, string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query =
                        "INSERT INTO client(client_name,client_email) VALUES(@Name,@Email)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Email", email);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // =====================================================
        // DELETE CLIENT
        // =====================================================
        public bool DeleteClient(int clientId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE client SET is_active = 0 WHERE client_id = @Id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Id", clientId);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
        }


        // =====================================================
        // GET ALL PROJECTS
        // =====================================================
        public List<ProjectModel> GetAllProjects()
        {
            List<ProjectModel> projects = new List<ProjectModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM project WHERE is_active = 1";

                    SqlCommand cmd = new SqlCommand(query, con);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        projects.Add(new ProjectModel
                        {
                            ProjectId = Convert.ToInt32(dr["project_id"]),
                            ProjectName = dr["project_name"].ToString(),
                            ClientId = Convert.ToInt32(dr["client_id"])
                        });
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return projects;
        }

        // =====================================================
        // INSERT PROJECT
        // =====================================================
        public bool InsertProject(string name, int clientId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query =
                        "INSERT INTO project(project_name,client_id) VALUES(@Name,@ClientId)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@ClientId", clientId);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // =====================================================
        // DELETE PROJECT
        // =====================================================
        public bool DeleteProject(int projectId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE project SET is_active = 0 WHERE project_id = @Id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Id", projectId);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
        }

        // =====================================================
        // GET ALL ACTIVITY TYPES
        // =====================================================
        public List<ActivityType> GetAllActivityTypes()
        {
            List<ActivityType> activities = new List<ActivityType>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM activity_type WHERE is_active = 1";

                    SqlCommand cmd = new SqlCommand(query, con);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        activities.Add(new ActivityType
                        {
                            ActivityTypeId = Convert.ToInt32(dr["activity_type_id"]),
                            ActivityName = dr["activity_type_name"].ToString()
                        });
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return activities;
        }

        // =====================================================
        // INSERT ACTIVITY TYPE
        // =====================================================
        public bool InsertActivityType(string activityName)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query =
                        "INSERT INTO activity_type(activity_type_name) VALUES(@Name)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Name", activityName);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // =====================================================
        // DELETE ACTIVITY TYPE
        // =====================================================
        public bool DeleteActivityType(int activityTypeId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE activity_type SET is_active = 0 WHERE activity_type_id = @Id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Id", activityTypeId);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception) { return false; }
        }
    }
}