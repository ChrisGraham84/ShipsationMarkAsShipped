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
    public static class StoreService
    {
        public static int GetEtsyStoreId(RestClient RestClient,string CompanyName)
        {
            List<Store> stores = new List<Store>();

            RestRequest request = new RestRequest("stores", Method.GET);
            //request.AddParameter("marketplaceId", 8);
            IRestResponse response = RestClient.Execute(request);
            foreach (var o in JArray.Parse(response.Content))
            {
                Store store = JsonConvert.DeserializeObject<Store>(o.ToString());
                stores.Add(store);
            }

            int storeId = stores.Where(x => x.companyName.ToLower() == CompanyName.ToLower()).FirstOrDefault().storeId;
            Thread.Sleep(1000);
            return (storeId);

        }
        public static List<Store> GetAllStores(RestClient RestClient)
        {
            List<Store> stores = new List<Store>();

            RestRequest request = new RestRequest("stores", Method.GET);
            IRestResponse response = RestClient.Execute(request);
            foreach (var o in JArray.Parse(response.Content))
            {
                Store store = JsonConvert.DeserializeObject<Store>(o.ToString());
                stores.Add(store);
            }
            Thread.Sleep(1000);
            return (stores);
        }
    }
}
