using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using ShipStationMarkAsShipped.Helpers;
using ShipStationMarkAsShipped.Model;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace ShipStationMarkAsShipped.Services
{
    public static class OrderServices
    {
        public static List<Order> GetOrders(RestClient RestClient, int StoreId)
        {
            List<Order> orders = new List<Order>();
            bool allOrdersRetrieved = false;
            int count = 0;
            int page = 1;
            int totalpage = 1;
            try
            {
                while(allOrdersRetrieved == false)
                {
                    RestRequest rrGetOrder = new RestRequest("orders", Method.GET);
                    rrGetOrder.AddParameter("orderStatus", "awaiting_shipment");
                    rrGetOrder.AddParameter("storeId", StoreId);
                    rrGetOrder.AddParameter("pageSize", "500");
                    rrGetOrder.AddParameter("page", page);
                    IRestResponse responseOrder = RestClient.Execute(rrGetOrder);
                    if (responseOrder.StatusCode == HttpStatusCode.OK)
                    {
                        JObject json = JObject.Parse(responseOrder.Content);
                        totalpage = int.Parse(json["page"].ToString());
                        foreach (var o in json["orders"])
                        {
                            Order order = JsonConvert.DeserializeObject<Order>(o.ToString());
                            orders.Add(order);
                        }
                    }

                    if(page == totalpage)
                    {

                    }
                }
               
            }
            catch (Exception ex)
            {

            }
            Thread.Sleep(1000);
            return (orders);
        }
        //Uses the summary order number to retrieve the order object from ShipStation API
        //so that the order object to be returned will have the orderId
        public static string GetOrderByOrderNumber(RestClient RestClient, ShippingSummary Summary)
        {
            Console.WriteLine("Grabbing Order by Order Number : {0}",Summary.Order_No);
            try
            {
                RestRequest rrGetOrder = new RestRequest("orders", Method.GET);
                rrGetOrder.AddParameter("orderNumber", Summary.Order_No);
                IRestResponse responseOrder = RestClient.Execute(rrGetOrder);
                JObject o = JObject.Parse(responseOrder.Content);
                int orderId = int.Parse(o["orders"][0]["orderId"].ToString());

                //make a new object to mark as shipped
                OrderPost order = new OrderPost {
                    orderId = orderId,
                    trackingNumber = Summary.Tracking,
                    shipDate = Summary.Ship_Date,
                    carrierCode = "usps",
                    notifyCustomer = true,
                    notifySalesChannel = true
                };
                Thread.Sleep(500);
                return (JsonConvert.SerializeObject(order));
            }
            catch(Exception ex)
            {
                return ("An Error Has Occured " + ex.ToString());
            }
        }

        //possible code to keep library consistency. TODO test with RestSharp
        public static bool MarkOrderPost(RestClient RestClient, string JSONRecord)
        {
            RestRequest request = new RestRequest("orders/markasshipped", Method.POST);
            request.AddHeader("Content-Type", "application/json");
            //request.RequestFormat = DataFormat.Json;
            //request.AddBody(summary);
            //request.AddJsonBody(JSONRecord);
            request.AddParameter("application/json; charset=utf-8", JSONRecord, ParameterType.RequestBody);
            try
            {
                IRestResponse response = RestClient.Execute(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Error Has Occured " + ex.ToString());
                return false;
            }
        }

        //used for posting the JSON order, Uses Net.HTTP
        public static async Task<bool> MakeAsyncOrderPost(string jsonRecord, string consumerKey, string consumerSecret)
        {
            Console.WriteLine("Posting Order to MarkedAsShipped");
            var baseAddress = new Uri("https://ssapi.shipstation.com/");

            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                string authInfo = consumerKey + ":" + consumerSecret;
                authInfo = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", authInfo);
                using (var content = new StringContent(jsonRecord, System.Text.Encoding.Default, "application/json"))
                {
                    var response = await httpClient.PostAsync("orders/markasshipped", content);

                    //needs to have some kind of error reporting so that it is known if anyting failed.
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("did work");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("did Not work");
                        return false;
                    }
                }
            }


        }
    }
}
