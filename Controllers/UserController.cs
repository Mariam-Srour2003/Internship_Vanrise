using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using MariamProject.Models;

public class UserController : Controller
{
    private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;

    public ActionResult Index()
    {
        return View();
    }

    public ActionResult AddUser()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Login(string username, string password)
    {
        bool userExists = CheckUserExists(username);

        if (userExists)
        {
            string hashedPasswordFromUser = HashPassword(password);

            string hashedPasswordFromDatabase = GetPasswordHash(username);

            if (hashedPasswordFromUser == hashedPasswordFromDatabase)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
            }
        }
        else
        {
            ViewBag.ErrorMessage = "User does not exist.";
        }
        return View("Index");
    }

    [HttpPost]
    public ActionResult AddUser(string username, string password)
    {
        string hashedPassword = HashPassword(password);
        bool userAdded = AddUserToDatabase(username, hashedPassword);

        if (userAdded)
        {
            return RedirectToAction("Index");
        }
        else
        {
            ViewBag.ErrorMessage = "Failed to add the user.";
            return View("AddUser");
        }
    }

    private bool CheckUserExists(string username)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand cmd = new SqlCommand("CheckUserExists", connection))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);

                SqlParameter userExistsParam = new SqlParameter("@UserExists", SqlDbType.Bit);
                userExistsParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(userExistsParam);

                cmd.ExecuteNonQuery();

                return Convert.ToBoolean(cmd.Parameters["@UserExists"].Value);
            }
        }
    }

    private string GetPasswordHash(string username)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand cmd = new SqlCommand("SELECT password_hash FROM users WHERE Username = @Username", connection))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }
    }

    private string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool AddUserToDatabase(string username, string hashedPassword)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand cmd = new SqlCommand("INSERT INTO users (username, password_hash) VALUES (@Username, @PasswordHash)", connection))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }
    }
}