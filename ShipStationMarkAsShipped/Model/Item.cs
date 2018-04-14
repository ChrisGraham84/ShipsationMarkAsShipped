using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipStationMarkAsShipped.Model
{
    public class Item
    {
        public int orderItemId { get; set; }
        public string lineItemKey { get; set; }
        public string sku { get; set; }
        public string name { get; set; }
        public string imageUrl { get; set; }
        public Weight weight { get; set; }
        public string quantity { get; set; }
        public string unitPrice { get; set; }
        public string taxAmount { get; set; }
        public string shippingAmount { get; set; }
        public string warehouseLocation { get; set; }
        public List<Option> options { get; set; }
        public string productId { get; set; }
        public string fulfillmentSku { get; set; }
        public string adjustment { get; set; }
        public string upc { get; set; }
        public string createDate { get; set; }
        public string modifyDate { get; set; }
    }
}
