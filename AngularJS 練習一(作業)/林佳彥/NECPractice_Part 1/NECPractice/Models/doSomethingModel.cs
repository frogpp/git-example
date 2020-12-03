using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
namespace NECPractice.Models
{
    public class doSomethingModel
    {
        private readonly string connStr = "Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\\Projects\\VSCode\\Q\\NECPractice\\App_Data\\TestDB.mdf;Integrated Security=True;MultipleActiveResultSets=True;Connect Timeout=30;Application Name=EntityFramework";

        public List<Vendor_D> getData() {
            List<Vendor_D> vens = new List<Vendor_D>();
            SqlConnection con = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand("select * from Vendor_D");
            cmd.Connection = con;
            con.Open();

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Vendor_D ven = new Vendor_D
                    {
                        SupId = reader.GetString(reader.GetOrdinal("SupId")),
                        FirmNo = reader.GetString(reader.GetOrdinal("FirmNo")),
                        FirmName = reader.GetString(reader.GetOrdinal("FirmName"))
                    };
                    vens.Add(ven);
                }
            }
            else
            {
                Console.WriteLine("db is null!!!!!");
            }
            con.Close();
            return vens;
        }

        public void AddData(Vendor_D ven)
        {
            SqlConnection con = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand(@"insert into Vendor_D (SupId, FirmNo, FirmName)"
            + "values(@SupId, @FirmNo, @FirmName)");
            cmd.Connection = con;

            cmd.Parameters.Add(new SqlParameter("@SupId", ven.SupId));
            cmd.Parameters.Add(new SqlParameter("@SupId", ven.FirmNo));
            cmd.Parameters.Add(new SqlParameter("@SupId", ven.FirmName));

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}