using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipStationMarkAsShipped.Model
{
    public class ShippingSummary
    {
        //public string CompanyId { get; set; }


        public string Company_ID { get; set; }
        public string Company_Name { get; set; }
        public string Order_No { get; set; }
        public string Carrier { get; set; }
        public string Ship_Type { get; set; }
        public string Ship_Date { get; set; }
        public string Ship_Cost { get; set; }
        public string Total_Weight { get; set; }
        public string Total_Boxes { get; set; }
        public string Tracking { get; set; }
        public string Item_Count { get; set; }
        public string Shipped { get; set; }
        public string PrimaryID { get; set; }
        public string Comments1 { get; set; }
        public string Garment_Deli_Coupon { get; set; }

        public ShippingSummary()
        {

        }
    }
}
