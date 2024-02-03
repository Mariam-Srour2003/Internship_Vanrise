using MariamProject.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

public class PhoneNumberReportController : Controller
{

    private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionDB"].ConnectionString;

    public ActionResult Index(string searchText)
    {
        List<PhoneNumberReport> phonenumbersreport = new List<PhoneNumberReport>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            using (SqlCommand command = new SqlCommand("GetPhoneNumbersperdevicereport", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PhoneNumberReport phonenumber = new PhoneNumberReport
                        {
                            Device = (int)reader["DeviceId"],
                            NumberOfPhoneNumbersUnReserved = (int)reader["nbunreserved"],
                            NumberOfPhoneNumbersReserved = (int)reader["nbreserved"],

                        };

                        phonenumbersreport.Add(phonenumber);
                    }
                    reader.Close();
                }
            }
            foreach (var phonenumber in phonenumbersreport)
            {
                phonenumber.DeviceName = GetDeviceName(connection, phonenumber.Device);
            }
        }
        if (!string.IsNullOrEmpty(searchText))
        {
            phonenumbersreport = phonenumbersreport.Where(phonenumber => phonenumber.DeviceName.Contains(searchText)).ToList();
        }

        return View(phonenumbersreport);
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

}
