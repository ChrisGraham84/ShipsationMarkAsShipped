using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipStationMarkAsShipped.Model
{
    public class OrderPost
    {
        public int orderId { get; set; }
        public string carrierCode { get; set; }
        public string shipDate { get; set; }
        public string trackingNumber { get; set; }
        public bool notifyCustomer { get; set; }
        public bool notifySalesChannel { get; set; }

        public OrderPost()
        {

        }
    }
}
