using ShipStationMarkAsShipped.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ShipStationMarkAsShipped.Helpers
{
    public static class ImportHelper
    {
        private static IEnumerable<DataRow> ReadFrom(string file, string sheet)
        {

            using (OleDbConnection con = new OleDbConnection(string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 8.0", file)))
            {
                OleDbDataAdapter da = new OleDbDataAdapter(string.Format("select * from [{0}$]", sheet), con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow dr in dt.AsEnumerable())
                {
                    yield return dr;

                }
            }

        }
        private static IEnumerable<DataRow> ReadFromLongColumn(string file, string sheet)
        {
            using (OleDbConnection con = new OleDbConnection(string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=No;IMEX=1""", file)))
            {
                OleDbDataAdapter da = new OleDbDataAdapter(string.Format("select * from [{0}$]", sheet), con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                DataTable dt2 = new DataTable();

                foreach (DataColumn dc in dt.Columns)
                {
                    dt2.Columns.Add(dt.Rows[0][dc.ColumnName].ToString());
                }
                foreach (DataRow drreal in dt.AsEnumerable().Skip(1))
                {
                    DataRow newRow = dt2.NewRow();
                    int count = 0;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        newRow[count] = drreal[dc];
                        count++;
                    }
                    dt2.Rows.Add(newRow);
                }

                foreach (DataRow dr in dt2.AsEnumerable())
                {
                    yield return dr;

                }
            }
        }
        private static List<string> Columns(string file, string sheet)
        {
            List<string> columnNames = new List<string>();
            using (OleDbConnection con = new OleDbConnection(string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 8.0;HDR=No;IMEX=1""", file)))
            {
                OleDbDataAdapter da = new OleDbDataAdapter(string.Format("select * from [{0}$]", sheet), con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataColumn dc in dt.Columns)
                {
                    columnNames.Add(dt.Rows[0][dc.ColumnName].ToString());
                }
            }
            return (columnNames);
        }

        public static List<ShippingSummary> ImportShippingSummaries(string Path)
        {
            List<ShippingSummary> summaries = new List<ShippingSummary>();

            string path = Path;
            string sheet = "Shipping_Summary___Optional_Sav";
            var data = ReadFrom(path, sheet);

            summaries = data.Select(x => new ShippingSummary
            {
                Company_ID = x["Company_ID"].ToString(),
                Order_No = x["Order_No"].ToString(),
                Tracking = x["Tracking"].ToString(),
                Ship_Date = DateTime.Parse(x["Ship_Date"].ToString()).ToString("yyyy-MM-dd"),
                Carrier = x["Carrier"].ToString().ToLower()

            }).ToList();

            return (summaries);
        }
        public static List<ShipstationCustomer> ImportShipstationCustomers()
        {
            List<ShipstationCustomer> customers = new List<ShipstationCustomer>();

            string line;
            using (StreamReader file = new StreamReader(System.Configuration.ConfigurationManager.AppSettings["Path"] + "shipstation_customers.txt"))
            {
                while ((line = file.ReadLine()) != null)
                {
                    List<string> sepList = new List<string>();
                    char[] delimiters = new char[] { '\t' };
                    string[] parts = line.Replace("\t", "\t ").Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                    ShipstationCustomer customer = new ShipstationCustomer();
                    customer.CompanyId = parts[0].Trim();
                    customer.CompanyName = parts[1].Trim();
                    customer.Key = parts[2].Trim();
                    customer.Token = parts[3].Trim();
                    customers.Add(customer);
                }
                file.Close();
            }
            return (customers);
        }
        public static List<Carrier> GetCarriers()
        {
            List<Carrier> carriers = new List<Carrier>();

            using (StreamReader reader = new StreamReader(System.Configuration.ConfigurationManager.AppSettings["Path"] + "carriers.js"))
            {
                string jsonText = reader.ReadToEnd();
                JArray json = JArray.Parse(jsonText);
                foreach (var c in json)
                {
                    Carrier carrier = JsonConvert.DeserializeObject<Carrier>(c.ToString());
                    carriers.Add(carrier);
                }
            }
            return (carriers);
        }

        public static List<Service> GetServices()
        {
            List<Service> services = new List<Service>();

            using (StreamReader reader = new StreamReader(System.Configuration.ConfigurationManager.AppSettings["Path"] + "services.js"))
            {
                string jsonText = reader.ReadToEnd();
                JArray json = JArray.Parse(jsonText);
                foreach (var c in json)
                {
                    Service service = JsonConvert.DeserializeObject<Service>(c.ToString());
                    services.Add(service);
                }
            }

            return (services);
        }

        public static List<T> GetObjectFromJsonArray<T>(string filename)
        {
            List<T> collection = new List<T>();
            using (StreamReader reader = new StreamReader(System.Configuration.ConfigurationManager.AppSettings["Path"] + filename))
            {
                string jsonText = reader.ReadToEnd();
                JArray json = JArray.Parse(jsonText);
                foreach (var c in json)
                {
                    T obj = JsonConvert.DeserializeObject<T>(c.ToString());
                    collection.Add(obj);
                }
            }
            return (collection);
        }
    }
}
