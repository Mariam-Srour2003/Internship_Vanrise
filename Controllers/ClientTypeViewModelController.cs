using MariamProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace MariamProject.Controllers
{
    public class ClientTypeViewModelController : Controller
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;

        public ActionResult Index(int? searchType)
        {
            List<ClientTypeViewModel> ClientTypeViews = new List<ClientTypeViewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("GetClientTypeView", connection))
                {
                    // Ensure the parameter name and data type match the stored procedure
                    command.Parameters.Add(new SqlParameter("@ClientType", SqlDbType.Int));

                    // Set the parameter value based on the searchType
                    command.Parameters["@ClientType"].Value = searchType.HasValue ? searchType.Value : (object)DBNull.Value;

                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClientTypeViewModel ClientTypeView = new ClientTypeViewModel
                            {
                                Type = (ClientType)reader["Type"],
                                TotalClients = (int)reader["ClientCount"],

                            };
                            ClientTypeViews.Add(ClientTypeView);
                        }
                    }
                }
            }

            return View(ClientTypeViews);
        }
    }
    }
