using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Collections.Generic;
using SampleProject.Models;


namespace SampleProject.DAL
{
    public class AuthenticationDAL
    {
        private string connectionString =
           ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        // ==============================
        // REGISTER USER
        // ==============================
        public bool Register(string username, string email, string password, int? managerId,int departmentId)
        {
            try
            {
                //verify dal data
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO employee
                    (name,email,password,manager_id,department_id,role)
                    VALUES(@UserName,@Email,@Password,@ManagerId,@DepartmentId,'Employee')";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@Email", email);
                    //proper hash creation
                    cmd.Parameters.AddWithValue("@Password", HashPassword(password));
                    cmd.Parameters.AddWithValue("@ManagerId",
                        (object)managerId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                    con.Open(); //check command and parameters
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        // ==============================
        // REGISTER Manager
        // ==============================
        public bool RegisterManger(string username, string email, string password, int? managerId,int departmentId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO employee
                    (name,email,password,manager_id,department_id,role)
                    VALUES(@UserName,@Email,@Password,@ManagerId,@DepartmentId,'Manager')";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@UserName", username);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", HashPassword(password));
                    cmd.Parameters.AddWithValue("@ManagerId",
                        (object)managerId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ==============================
        // LOGIN USER
        // ==============================
        public Employee Login(string email, string password)
        {
            try
            {//check parameters of login
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query =
                        "SELECT employee_id,name,email,password,role,manager_id FROM employee WHERE email=@Email AND is_active = 1";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Email", email);

                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())//check if it has rows
                        {
                            string storedHash = reader["password"].ToString();

                            if (VerifyPassword(password, storedHash))//check result
                            {
                                return new Employee
                                {
                                    UserId = Convert.ToInt32(reader["employee_id"]),
                                    UserName = reader["name"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Role = reader["role"].ToString(),
                                    ManagerId = reader["manager_id"] == DBNull.Value
                                        ? (int?)null
                                        : Convert.ToInt32(reader["manager_id"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        // ==============================
        // HASH PASSWORD
        // ==============================
        private string HashPassword(string password)
        {
            byte[] salt = new byte[16];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);//check generated salt
            }

            var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                100000,
                HashAlgorithmName.SHA256);

            byte[] hash = pbkdf2.GetBytes(32);//check generated hash

            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);//check hashbyte
        }

        // ==============================
        // VERIFY PASSWORD
        // ==============================
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);//check

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);//check extracted salt

            var pbkdf2 = new Rfc2898DeriveBytes(
                enteredPassword,
                salt,
                100000,
                HashAlgorithmName.SHA256);

            byte[] hash = pbkdf2.GetBytes(32);//check hash

            for (int i = 0; i < 32; i++)//check mismatch
                if (hashBytes[i + 16] != hash[i])
                    return false;

            return true;
        }

        // ==============================
        // SEED DEFAULT MANAGERS
        // ==============================
        public void SeedManagers()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string checkQuery =
                        "SELECT COUNT(*) FROM employee WHERE role='Manager'";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        string query = @"INSERT INTO employee
                        (name,email,password,manager_id,role)
                        VALUES(@Name,@Email,@Password,NULL,'Manager')";

                        string[,] managers =
                        {
                            {"Rahul","rahul@system.com"},
                            {"Anita","anita@system.com"},
                            {"Vikram","vikram@system.com"},
                            {"Meera","meera@system.com"}
                        };

                        for (int i = 0; i < 4; i++)
                        {
                            SqlCommand cmd = new SqlCommand(query, con);

                            cmd.Parameters.AddWithValue("@Name", managers[i, 0]);
                            cmd.Parameters.AddWithValue("@Email", managers[i, 1]);
                            cmd.Parameters.AddWithValue("@Password",
                                HashPassword("Manager@123"));

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        // ==============================
        // GET MANAGERS
        // ==============================
        public List<Employee> GetManagers()
        {
            List<Employee> list = new List<Employee>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query =
                        "SELECT employee_id,name FROM employee WHERE role='Manager' AND is_active = 1";

                    SqlCommand cmd = new SqlCommand(query, con);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        list.Add(new Employee
                        {
                            UserId = Convert.ToInt32(dr["employee_id"]),
                            UserName = dr["name"].ToString()
                        });
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return list;
        }
        // ==============================
        // GET USER PROFILE
        // ==============================
        public Employee GetUserProfile(int userId)
        {
            try
            {//check user id from controller
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query =
                        "SELECT employee_id,name,email FROM employee WHERE employee_id=@UserId";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    con.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())//check rows
                    {
                        return new Employee
                        {
                            UserId = Convert.ToInt32(dr["employee_id"]),
                            UserName = dr["name"].ToString(),
                            Email = dr["email"].ToString()
                        };
                    }
                }
            }
            catch {
                return null;
            }
            return null;

        }
        // ==============================
        // SEED DEFAULT ADMIN
        // ==============================
        public void SeedAdmin()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    
                    string checkQuery =
                        "SELECT COUNT(*) FROM employee WHERE role='Admin'";

                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    int count = (int)checkCmd.ExecuteScalar();

                   
                    if (count == 0)
                    {
                        string query = @"INSERT INTO employee
                (name,email,password,manager_id,role)
                VALUES(@Name,@Email,@Password,NULL,'Admin')";

                        SqlCommand cmd = new SqlCommand(query, con);

                        cmd.Parameters.AddWithValue("@Name", "System Admin");
                        cmd.Parameters.AddWithValue("@Email", "admin@system.com");
                        cmd.Parameters.AddWithValue("@Password",
                            HashPassword("Admin@123"));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }
        // ==============================
        // UPDATE USER PROFILE (NAME / EMAIL / PASSWORD)
        // ==============================
        public bool UpdateUserProfile(int userId, string name, string email, string newPassword)
        {
            try
            {//ckeck details from controller
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query;

                    
                    if (!string.IsNullOrWhiteSpace(newPassword))//check null condition
                    {
                        query = @"UPDATE employee
                          SET name = @Name,
                              email = @Email,
                              password = @Password
                          WHERE employee_id = @UserId";
                    }
                    else
                    {
                        
                        query = @"UPDATE employee
                          SET name = @Name,
                              email = @Email
                          WHERE employee_id = @UserId";
                    }

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Email", email);

                  
                    if (!string.IsNullOrWhiteSpace(newPassword))
                    {
                        string hashedPassword = HashPassword(newPassword);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    }

                    return cmd.ExecuteNonQuery() > 0;//check result
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        // ==============================
        // GET DEPARTMENTS
        // ==============================
        public List<DepartmentModel> GetDepartments()
        {
            List<DepartmentModel> list = new List<DepartmentModel>();

            try//check function call
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query =
                        "SELECT department_id, department_name FROM department";

                    SqlCommand cmd = new SqlCommand(query, con);

                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())//check rows
                    {
                        list.Add(new DepartmentModel
                        {
                            DepartmentId = Convert.ToInt32(dr["department_id"]),
                            DepartmentName = dr["department_name"].ToString()
                        });
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return list;//check result
        }
        // ==============================
        // GET USER ID BY EMAIL
        // ==============================
        public int GetUserIdByEmail(string email)
        {
            try
            {//check function call
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query =
                        "SELECT employee_id FROM employee WHERE email=@Email";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Email", email);

                    con.Open();

                    object result = cmd.ExecuteScalar();

                    return result == null ? 0 : Convert.ToInt32(result);
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }

}