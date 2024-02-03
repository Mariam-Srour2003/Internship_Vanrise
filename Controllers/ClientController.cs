using MariamProject.Models;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;

public class ClientController : Controller
{
    private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;

    public ActionResult Index(string searchText, int? searchType)
    {
        List<Client> clients = new List<Client>();
        


        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetAllClients", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Client client = new Client
                        {
                            ClientId = (int)reader["Id"],
                            ClientName = reader["Name"].ToString(),
                            ClientType = (ClientType)reader["Type"],
                            ClientBirthday = reader["Birthday"] == DBNull.Value ? null : (DateTime?)reader["Birthday"],
                            ZoneId = (int)reader["ZoneId"]
                        };

                        clients.Add(client);
                    }

                }
            }
            foreach (var client in clients)
            {
                client.ReservedPhoneNumbersId = GetReservations(connection, client.ClientId);
                client.ZoneName = GetZoneName(connection, client.ZoneId);
            }
        }
        Dictionary<int, int> reservationsCount = GetReservationsCountForClients(clients);

        ViewBag.Clients = clients;
        ViewBag.ReservationsCount = reservationsCount;
        if (!string.IsNullOrEmpty(searchText))
        {
            clients = clients.Where(client =>
                client.ClientName.Contains(searchText)).ToList();
        }

        if (searchType.HasValue && searchType.Value != 0)
        {
            clients = clients.Where(client => (int)client.ClientType == searchType.Value).ToList();
        }

        return View(clients);
    }

    private string GetZoneName(SqlConnection connection, int zoneid)
    {
        using (SqlCommand command = new SqlCommand("GetZoneName", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ZoneId", zoneid);

            return (string)command.ExecuteScalar();
        }
    }

    private List<int> GetReservations(SqlConnection connection, int clientId)
    {
        List<int> reservedPhoneNumbers = new List<int>();

        using (SqlCommand command = new SqlCommand("GetReservationsByClientId", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ClientId", clientId);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var PhoneNumberId = (int)reader["Id"];

                    reservedPhoneNumbers.Add(PhoneNumberId);
                }
                reader.Close();
            }
        }

        return reservedPhoneNumbers;
    }
    [HttpGet]
    public List<int> GetReservations(int clientId)
    {
        List<int> reservedPhoneNumbers = new List<int>();
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
                        var phoneNumberId = (int)reader["PhoneNumberID"];
                        reservedPhoneNumbers.Add(phoneNumberId);
                    }
                    reader.Close();
                }
            }
        }

        return reservedPhoneNumbers;
    }

    [HttpPost]
    public ActionResult AddClient(string newName, DateTime? Birthday, int Type, int newZone, int newSite)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("AddaNewClient", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@NewName", newName);
                command.Parameters.AddWithValue("@Type", Type);
                command.Parameters.AddWithValue("@BirthDay", Birthday ?? (object)DBNull.Value); // Handle null
                command.Parameters.AddWithValue("@ZoneId", newZone);
                command.Parameters.AddWithValue("@SiteId", newSite);
                command.ExecuteNonQuery();
            }
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public ActionResult EditClient(int clientId, string newName, int newType, DateTime? newBirthday, int newZone, int newSite)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("EditaClient", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ClientId", clientId);
                command.Parameters.AddWithValue("@NewName", newName);
                command.Parameters.AddWithValue("@NewType", newType);
                command.Parameters.AddWithValue("@NewBirthDay", newBirthday ?? (object)DBNull.Value); // Handle null
                command.Parameters.AddWithValue("@NewZoneId", newZone);
                command.Parameters.AddWithValue("@NewSiteId", newSite);
                command.ExecuteNonQuery();
            }
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public ActionResult AddReservation(int clientId, int phoneNumberId)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("AddPhoneNumberReservation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ClientID", clientId);
                    command.Parameters.AddWithValue("@PhoneNumberID", phoneNumberId);
                    command.ExecuteNonQuery();
                }
            }

            return Json(new { success = true, message = "Reservation added successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error adding reservation: " + ex.Message });
        }
    }
    public Dictionary<int, int> GetReservationsCountForClients(List<Client> clients)
    {
        Dictionary<int, int> reservationsCount = new Dictionary<int, int>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            foreach (var client in clients)
            {
                using (SqlCommand command = new SqlCommand("GetReservationsCountByClientId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ClientId", client.ClientId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int count = (int)reader["ReservationCount"];
                            reservationsCount.Add(client.ClientId, count);
                        }
                    }
                }
            }
        }

        return reservationsCount;
    }

    [HttpPost]
    public ActionResult Unreserve(int clientId, int phoneNumberId)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("UnreservePhoneNumberReservation", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ClientID", clientId);
                    command.Parameters.AddWithValue("@PhoneNumberID", phoneNumberId);
                    command.ExecuteNonQuery();
                }
            }

            return Json(new { success = true, message = "Reservation unreserved successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error unreserving reservation: " + ex.Message });
        }
    }


}
