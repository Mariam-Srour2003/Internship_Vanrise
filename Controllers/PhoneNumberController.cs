using MariamProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls.Expressions;

public class PhoneNumberController : Controller
{
    private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;

    public ActionResult Index(string searchText, int? searchDeviceId)
    {
        List<PhoneNumber> phonenumbers = new List<PhoneNumber>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetPhoneNumbers", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PhoneNumber phonenumber = new PhoneNumber
                        {
                            PhoneNumberId = (int)reader["Id"],
                            Number = reader["Number"].ToString(),
                            DeviceId = (int)reader["DeviceId"],
                        };

                        phonenumbers.Add(phonenumber);
                    }
                    reader.Close();
                }
            }
            foreach (var phonenumber in phonenumbers)
            {
                phonenumber.DeviceName = GetDeviceName(connection, phonenumber.DeviceId);
            }
        }

        if (searchDeviceId.HasValue && searchDeviceId != 0)
        {
            phonenumbers = phonenumbers.Where(phonenumber => phonenumber.DeviceId == searchDeviceId).ToList();
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            phonenumbers = phonenumbers.Where(phonenumber =>
                phonenumber.Number.Contains(searchText)).ToList();
        }

        return View(phonenumbers);
    }


    public ActionResult GetPhoneNumbers(string searchText, int? searchDeviceId)
    {
        List<PhoneNumber> phonenumbers = new List<PhoneNumber>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetPhoneNumbers", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PhoneNumber phonenumber = new PhoneNumber
                        {
                            PhoneNumberId = (int)reader["Id"],
                            Number = reader["Number"].ToString(),
                            DeviceId = (int)reader["DeviceId"],
                        };

                        phonenumbers.Add(phonenumber);
                    }
                    reader.Close();
                }
            }
            foreach (var phonenumber in phonenumbers)
            {
                phonenumber.DeviceName = GetDeviceName(connection, phonenumber.DeviceId);
            }
        }

        if (searchDeviceId.HasValue && searchDeviceId != 0)
        {
            phonenumbers = phonenumbers.Where(phonenumber => phonenumber.DeviceId == searchDeviceId).ToList();
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            phonenumbers = phonenumbers.Where(phonenumber =>
                phonenumber.Number.Contains(searchText)).ToList();
        }

        return PartialView("_PhoneNumbersTable", phonenumbers);
    }


    private string GetDeviceName(SqlConnection connection, int deviceId)
    {
        using (SqlCommand command = new SqlCommand("GetDeviceName", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DeviceId", deviceId);

            return (string)command.ExecuteScalar();
        }
    }

    [HttpPost]
    public ActionResult AddPhoneNumber(string newNumber, int deviceid)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("AddPhoneNumber", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@DeviceID", deviceid);
                command.Parameters.AddWithValue("@Number", newNumber);
                command.ExecuteNonQuery();
            }
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public ActionResult UpdatePhoneNumberNumber(int id, string newNumber, int deviceid)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("UpdatePhoneNumber", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PhoneNumberId", id);
                command.Parameters.AddWithValue("@Number", newNumber);
                command.Parameters.AddWithValue("@DeviceId", deviceid);
                command.ExecuteNonQuery();
            }
        }

        return Json(new { success = true });
    }
    [HttpGet]
    public ActionResult GetAllPhoneNumbers()
    {
        List<PhoneNumber> phonenumberstochoosefromthem = new List<PhoneNumber>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetPhoneNumbers", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PhoneNumber phonenumber = new PhoneNumber
                        {
                            PhoneNumberId = (int)reader["Id"],
                            Number = reader["Number"].ToString(),
                        };

                        phonenumberstochoosefromthem.Add(phonenumber);
                    }
                }
            }
        }

        return Json(phonenumberstochoosefromthem, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public ActionResult GetReservedPhoneNumbersForClient(int clientId)
    {
        try
        {
            List<PhoneNumber> reservedPhoneNumbers = new List<PhoneNumber>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("GetReservationsByClientId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ClientId", clientId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PhoneNumber phoneNumber = new PhoneNumber
                            {
                                PhoneNumberId = (int)reader["Id"],
                                Number = reader["Number"].ToString()
                            };

                            reservedPhoneNumbers.Add(phoneNumber);
                        }
                    }
                }
            }

            return Json(reservedPhoneNumbers, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            return Json(new { success = false, message = ex.Message });
        }
    }
    [HttpGet]
    public ActionResult GetUnreservedPhoneNumbers(int clientId)
    {
        List<PhoneNumber> UnreservedPhoneNumbers = new List<PhoneNumber>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetUnreservedPhoneNumbers", connection))
            {
                command.CommandType = CommandType.StoredProcedure; 

                command.Parameters.Add(new SqlParameter("@ClientId", clientId));

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PhoneNumber phonenumber = new PhoneNumber
                        {
                            PhoneNumberId = (int)reader["Id"],
                            Number = reader["Number"].ToString(),
                            DeviceId = (int)reader["DeviceId"],
                        };

                        UnreservedPhoneNumbers.Add(phonenumber);
                    }
                }
            }
        }

        return Json(UnreservedPhoneNumbers, JsonRequestBehavior.AllowGet);
    }

}
