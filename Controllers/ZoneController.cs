using MariamProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

public class ZoneController : Controller
{
    private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;

    public ActionResult Index(string searchText)
    {
        List<Zone> zones = new List<Zone>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetAllZone", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Zone zone = new Zone
                        {
                            ZoneId = (int)reader["id"],
                            ZoneName = reader["name"].ToString()
                        };
                        zones.Add(zone);
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            zones = zones.Where(zone => zone.ZoneName.Contains(searchText)).ToList();
        }

        return View(zones);
    }
    public ActionResult AddZone(string newName)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("AddNewZone", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ZoneName", newName);
                command.ExecuteNonQuery();
            }
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public ActionResult UpdateZoneName(int id, string newName)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("UpdateZoneName", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ZoneID", id);
                string name = newName;
                command.Parameters.AddWithValue("@NewName", name);
                command.ExecuteNonQuery();
            }
        }

        return Json(new { success = true });
    }

    public ActionResult GetZones(string searchText)
    {
        List<Zone> zones = new List<Zone>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetAllZone", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Zone zone = new Zone
                        {
                            ZoneId = (int)reader["id"],
                            ZoneName = reader["name"].ToString()
                        };
                        zones.Add(zone);
                    }
                }
            }
        }
        return Json(zones, JsonRequestBehavior.AllowGet); 
    }

}
