using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using MariamProject.Models;

public class PhoneNumberReservationController : Controller
{
    private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;

    public ActionResult Index()
    {
        List<PhoneNumberReservation> reservations = new List<PhoneNumberReservation>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetAllPhoneNumberReservation", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PhoneNumberReservation reservation = new PhoneNumberReservation
                        {
                            ReservationID = (int)reader["ReservationID"],
                            ClientID = (int)reader["ClientID"],
                            PhoneNumberID = (int)reader["PhoneNumberID"],
                            BED = (DateTime)reader["BED"],
                            EED = reader["EED"] as DateTime? 
                        };

                        reservations.Add(reservation);
                    }
                }
            }
            foreach (var reservation in reservations)
            {
                reservation.ClientName = GetClientName(connection, reservation.ClientID);
                reservation.Number = GetNumber(connection, reservation.PhoneNumberID);
            }
        }

        return View(reservations);
    }
    public ActionResult GetPhoneNumbersReservations(string searchText, int? searchClientId)
    {
        List<PhoneNumberReservation> reservations = new List<PhoneNumberReservation>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetAllPhoneNumberReservation", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PhoneNumberReservation reservation = new PhoneNumberReservation
                        {
                            ReservationID = (int)reader["ReservationID"],
                            ClientID = (int)reader["ClientID"],
                            PhoneNumberID = (int)reader["PhoneNumberID"],
                            BED = (DateTime)reader["BED"],
                            EED = reader["EED"] as DateTime?
                        };

                        reservations.Add(reservation);
                    }
                }
            }
            foreach (var reservation in reservations)
            {
                reservation.ClientName = GetClientName(connection, reservation.ClientID);
                reservation.Number = GetNumber(connection, reservation.PhoneNumberID);
            }
        }

        if (searchClientId.HasValue && searchClientId != 0)
        {
            reservations = reservations.Where(reservation => reservation.ClientID == searchClientId).ToList();
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            reservations = reservations.Where(reservation =>
                reservation.Number.Contains(searchText)).ToList();
        }

        // Convert the list of PhoneNumber objects to JSON and return it as an IActionResult.
        return PartialView("_ReservationsTable", reservations);
    }
    private string GetClientName(SqlConnection connection, int clientId)
    {
        using (SqlCommand command = new SqlCommand("GetClientName", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@ClientId", clientId);

            return (string)command.ExecuteScalar();
        }
    }
    private string GetNumber(SqlConnection connection, int phonenumberId)
    {
        using (SqlCommand command = new SqlCommand("GetNumber", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@PhoneNumberID", phonenumberId);

            return (string)command.ExecuteScalar();
        }
    }
}
