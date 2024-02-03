using MariamProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

public class DeviceController : Controller
{
    private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;

    public ActionResult Index(string searchText)
    {
        List<Device> devices = new List<Device>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetDevices", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Device device = new Device
                        {
                            DeviceId = (int)reader["Id"],
                            DeviceName = reader["Name"].ToString(),
                            ZoneId = (int)reader["ZoneId"],
                            SiteId = (int)reader["SiteId"]
                        };
                        devices.Add(device);
                    }
                }
            }
            foreach (var device in devices)
            {
                device.ZoneName = GetZoneName(connection, device.ZoneId);
                device.SiteName = GetSiteName(connection, device.SiteId);
            }
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            devices = devices.Where(device => device.DeviceName.Contains(searchText)).ToList();
        }

        return View(devices);
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


    private string GetSiteName(SqlConnection connection, int siteid)
    {
        using (SqlCommand command = new SqlCommand("GetSiteName", connection))
        {
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@SiteId", siteid);

            return (string)command.ExecuteScalar();
        }
    }
    [HttpPost]
    public ActionResult AddDevice(string newName, int zoneId, int siteId)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("AddNewDevice", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@NewName", newName);
                command.Parameters.AddWithValue("@ZoneId", zoneId);
                command.Parameters.AddWithValue("@SiteId", siteId);
                command.ExecuteNonQuery();
            }
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public ActionResult UpdateDeviceName(int id, string newName, int newZone, int newSite)
    {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("UpdateDeviceName", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DeviceID", id);
                    string name = newName;
                    command.Parameters.AddWithValue("@NewName", name);
                    command.Parameters.AddWithValue("@NewZoneId", newZone);
                    command.Parameters.AddWithValue("@NewSiteId", newSite);
                    command.ExecuteNonQuery();
                }
            }

            return Json(new { success = true });
    }
    [HttpGet]
    public ActionResult GetAllDevices()
    {
        List<Device> devicestochoosefromthem = new List<Device>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetDevices", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Device device = new Device
                        {
                            DeviceId = (int)reader["Id"],
                            DeviceName = reader["Name"].ToString(),
                            ZoneId = (int)reader["ZoneId"]
                        };

                        devicestochoosefromthem.Add(device);
                    }
                }
            }
        }

        return Json(devicestochoosefromthem, JsonRequestBehavior.AllowGet);
    }
}
