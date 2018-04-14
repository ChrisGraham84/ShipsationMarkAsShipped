using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipStationMarkAsShipped.Model
{
    public class Order
    {
        public int orderId { get; set; }
        public string orderNumber { get; set; }
        public string orderKey { get; set; }
        public string orderDate { get; set; }
        public string createDate { get; set; }
        public string modifyDate { get; set; }
        public string paymentDate { get; set; }
        public string shipByDate { get; set; }
        public string orderStatus { get; set; }
        public string customerId { get; set; }
        public string customerUsername { get; set; }
        public string customerEmail { get; set; }
        public BillTo billTo { get; set; }
        public ShipTo shipTo { get; set; }
        public List<Item> items { get; set; }
        public string orderTotal { get; set; }
        public string amountPaid { get; set; }
        public string taxAmount { get; set; }
        public string shippingAmount { get; set; }
        public string customerNotes { get; set; }
        public string internalNotes { get; set; }
        public bool gift { get; set; }
        public string giftMessage { get; set; }
        public string paymentMethod { get; set; }
        public string requestedShippingService { get; set; }
        public string carrierCode { get; set; }
        public string serviceCode { get; set; }
        public string packageCode { get; set; }
        public string confirmation { get; set; }
        public string shipDate { get; set; }
        public string holdUntilDate { get; set; }
        public Weight2 weight { get; set; }
        public Dimensions dimensions { get; set; }
        public InsuranceOptions insuranceOptions { get; set; }
        public InternationalOptions internationalOptions { get; set; }
        public AdvancedOptions advancedOptions { get; set; }
        public List<string> tagIds { get; set; }
        public string userId { get; set; }
        public string externallyFulfilled { get; set; }
        public string externallyFulfilledBy { get; set; }
    }
}
