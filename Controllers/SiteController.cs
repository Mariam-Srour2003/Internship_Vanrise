using MariamProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

public class SiteController : Controller
{
    private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;

    public ActionResult Index(string searchText)
    {
        List<Site> sites = new List<Site>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetAllSite", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Site site = new Site
                        {
                            SiteId = (int)reader["id"],
                            SiteName = reader["name"].ToString()
                        };
                        sites.Add(site);
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(searchText))
        {
            sites = sites.Where(site => site.SiteName.Contains(searchText)).ToList();
        }

        return View(sites);
    }
    public ActionResult AddSite(string newName)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("AddNewSite", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SiteName", newName);
                command.ExecuteNonQuery();
            }
        }

        return Json(new { success = true });
    }

    [HttpPost]
    public ActionResult UpdateSiteName(int id, string newName)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("UpdateSiteName", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@SiteID", id);
                string name = newName;
                command.Parameters.AddWithValue("@NewName", name);
                command.ExecuteNonQuery();
            }
        }

        return Json(new { success = true });
    }

    public ActionResult GetSitesByZoneId(int? zoneId)
    {
        try
        {
            List<Site> sites = new List<Site>();

            if (zoneId.HasValue) 
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("GetSitesByZoneId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ZoneId", zoneId.Value)); 
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Site site = new Site
                                {
                                    SiteId = (int)reader["Id"],
                                    SiteName = reader["Name"].ToString(),
                                };
                                sites.Add(site);
                            }
                        }
                    }
                }
            }

            return Json(sites, JsonRequestBehavior.AllowGet);
        }
        catch (Exception ex)
        {
            return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
        }
    }

}

