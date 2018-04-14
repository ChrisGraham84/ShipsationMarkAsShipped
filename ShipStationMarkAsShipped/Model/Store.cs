using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipStationMarkAsShipped.Model
{
    public class StatusMapping
    {
        public string orderStatus { get; set; }
        public string statusKey { get; set; }
    }

    public class Store
    {
        public int storeId { get; set; }
        public string storeName { get; set; }
        public int marketplaceId { get; set; }
        public string marketplaceName { get; set; }
        public string accountName { get; set; }
        public string email { get; set; }
        public string integrationUrl { get; set; }
        public bool active { get; set; }
        public string companyName { get; set; }
        public string phone { get; set; }
        public string publicEmail { get; set; }
        public string website { get; set; }
        public string refreshDate { get; set; }
        public string lastRefreshAttempt { get; set; }
        public string createDate { get; set; }
        public string modifyDate { get; set; }
        public string autoRefresh { get; set; }
        public List<StatusMapping> statusMappings { get; set; }
    }
}
