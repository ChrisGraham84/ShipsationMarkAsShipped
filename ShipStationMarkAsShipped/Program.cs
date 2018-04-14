using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using ShipStationMarkAsShipped.Services;

using System.Net.Http;
using Newtonsoft.Json;
using ShipStationMarkAsShipped.Helpers;
using ShipStationMarkAsShipped.Model;
using RestSharp;
using RestSharp.Authenticators;

using Newtonsoft.Json.Linq;
using System.IO;

namespace ShipStationMarkAsShipped
{
    class Program
    {
        static void Main(string[] args)
        {
          
            if (args.Count() > 0)
            {
                if (args[0] == "ship")
                {
                    #region Mark As Shipped
                    try
                    {
                        //This is a fix for it being a desktop application.  This should not 
                        //be run on a live webserver
                        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                        //Use app.config so the directory can be changed regardless of the code
                        string dir = ConfigurationManager.AppSettings["ShippingData"].ToString();

                        //grab all the files in the directory
                        Directory.GetFiles(dir);
                        foreach (var file in Directory.GetFiles(dir))
                        {
                            //Get just the name of the file
                            FileInfo import = new FileInfo(file);
                            string filename = import.Name.Split('.')[0];

                            //Split on '_' and it should create a array with the length of 3
                            string[] filenamesplit = filename.Split('_');
                            if (filenamesplit.Length == 4)
                            {
                                string consumerKey = filenamesplit[0];
                                string consumerSecret = filenamesplit[1];

                                var client = new RestClient();
                                client.BaseUrl = APIHelper.GetBaseUri();
                                client.Authenticator = new HttpBasicAuthenticator(consumerKey, consumerSecret);

                                var summaries = ImportHelper.ImportShippingSummaries(file);


                                if (summaries.FirstOrDefault() != null && summaries.FirstOrDefault().Company_ID != "-1")
                                {
                                    //changed to false if any of the orders fail to be posted.
                                    bool allShipped = true;
                                    foreach (var summary in summaries)
                                    {
                                        //Use the summary information to create a JSON object
                                        string order = OrderServices.GetOrderByOrderNumber(client, summary);

                                        //if the string did not return and error try to 
                                        //do a post.
                                        if (!order.StartsWith("An Error Has Occured"))
                                        {
                                            //Shipsation async code, but it causes business logic problems
                                            //Task tsk = OrderServices.MakeAsyncOrderPost(order, consumerKey, consumerSecret);
                                            //tsk.Wait();

                                            //Post the order to be marked.
                                            allShipped = OrderServices.MarkOrderPost(client, order);
                                        }
                                        else
                                        {
                                            allShipped = false;
                                        }
                                    }

                                    if (allShipped)
                                    {
                                        // File.Delete(file);
                                        //Update first row companyID to -1
                                        string DSN = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=Excel 8.0", file);
                                        using (System.Data.OleDb.OleDbConnection Con = new System.Data.OleDb.OleDbConnection(DSN))
                                        {
                                            Con.Open();
                                            using (System.Data.OleDb.OleDbCommand Com = new System.Data.OleDb.OleDbCommand())
                                            {
                                                Com.Connection = Con;
                                                Com.CommandText = "UPDATE [Shipping_Summary___Optional_Sav$] SET [Company_ID] = -1 WHERE [Company_ID] = " + summaries.FirstOrDefault().Company_ID;
                                                Com.ExecuteNonQuery();

                                            }
                                            Con.Close();
                                        }
                                    }
                                    else
                                    {
                                        MailMessage mail = new MailMessage();
                                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                                        mail.From = new MailAddress("resources@email.com");
                                        mail.To.Add("resources@email.com");
                                        mail.Subject = "An Error Has Occured Posting to Ship Station";
                                        mail.Body = string.Format("Not all orders for the file {0} were marked as shipped", import.Name);

                                        SmtpServer.Port = 587;
                                        SmtpServer.Credentials = new NetworkCredential("resources@email.com", "5HB7c9ut");
                                        SmtpServer.EnableSsl = true;

                                        SmtpServer.Send(mail);
                                    }
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                        mail.From = new MailAddress("resources@email.com");
                        mail.To.Add("resources@email.com");
                        mail.Subject = "An Error Has Occured Posting to Ship Station";
                        mail.Body = string.Format(ex.ToString());

                        SmtpServer.Port = 587;
                        SmtpServer.Credentials = new System.Net.NetworkCredential("resources@email.com", "5HB7c9ut");
                        SmtpServer.EnableSsl = true;

                        SmtpServer.Send(mail);
                    }
                    #endregion
                }

                if (args[0] == "orders")
                {
                    #region Pull Order
                    var customers = ImportHelper.ImportShipstationCustomers();
                    foreach (var customer in customers)
                    {
                        Console.WriteLine("Processing {0}", customer.CompanyName);
                        var client = new RestClient();
                        client.BaseUrl = APIHelper.GetBaseUri();
                        client.Authenticator = new HttpBasicAuthenticator(customer.Key, customer.Token);


                        //RestRequest request = new RestRequest("users", Method.GET);
                        //IRestResponse response = client.Execute(request);
                        //get all the stores
                        Console.WriteLine("Getting All Stores");
                        List<Store> stores = StoreService.GetAllStores(client);

                        Console.WriteLine("Pulling down unshipped orders");
                        //var orders = OrderServices.GetOrders(client, storeId);
                        List<Order> orders = new List<Order>();
                        foreach(var store in stores)
                        {
                            Console.WriteLine("Pulling down unshipped orders for {0}", store.storeName);
                            orders.AddRange(OrderServices.GetOrders(client, store.storeId));
                        }


                        var SClientorders = orders.Where(o => o.userId == APIHelper.GetShipstationUserID()).ToList();

                        //check to see if the id's have been 
                        List<string> extractedids = new List<string>();
                        using (StreamReader reader = new StreamReader(System.Configuration.ConfigurationManager.AppSettings["Path"] + "extractedids.txt"))
                        {
                            string line = reader.ReadLine();
                            while (line != null)
                            {
                                extractedids.Add(line.Trim());
                                line = reader.ReadLine();
                            }

                            reader.Close();
                        }
                        foreach (var id in extractedids)
                        {
                            var order = SClientorders.Where(xo => xo.orderNumber == id).FirstOrDefault();
                            if (order != null)
                            {
                                SClientorders.Remove(order);
                            }
                        }


                        if (SClientorders.Count > 0)
                        {
                            Console.WriteLine("Creating Extraction Report Count : {0}",SClientorders.Count);
                            ExportHelpers.ExtractionReport(SClientorders, customer,stores);

                            Console.WriteLine("Sending Email");
                            MailHelpers.SendEmailWithOrders(customer.CompanyName);
                            Console.WriteLine("Finished Processing {0}", customer.CompanyName);

                            //save the id's
                            using (StreamWriter writer = new StreamWriter(System.Configuration.ConfigurationManager.AppSettings["Path"] + "extractedids.txt", true))
                            {
                                
                                foreach(var order in SClientorders.OrderBy(y=>y.orderNumber))
                                {
                                    writer.WriteLine(order.orderNumber);
                                }
                            }
                        }
                        else
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                            mail.From = new MailAddress("resources@email.com");
                            mail.To.Add("admin@email.com,admin2@email.com");
                            mail.Bcc.Add("resources@email.com");
                            mail.Subject = customer.CompanyName + " Orders From Ship Station - No New Orders";
                            mail.Body = string.Format("No New Orders");

                            SmtpServer.Port = 587;
                            SmtpServer.Credentials = new System.Net.NetworkCredential("resources@email.com", "5HB7c9ut");
                            SmtpServer.EnableSsl = true;

                            SmtpServer.Send(mail);
                        }
                       
                    }
                    #endregion
                }
                Console.WriteLine("done");
            }
            else
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("resources@email.com");
                mail.To.Add("resources@email.com");
                mail.Subject = "An Error Has Occured Posting to Ship Station";
                mail.Body = string.Format("No Argument Has Been Passed");

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("resources@email.com", "5HB7c9ut");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
        }

    }
}
