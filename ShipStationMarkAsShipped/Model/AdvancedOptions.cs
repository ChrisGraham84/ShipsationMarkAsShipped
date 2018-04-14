using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipStationMarkAsShipped.Model
{
    public class AdvancedOptions
    {
        public string warehouseId { get; set; }
        public bool nonMachinable { get; set; }
        public bool saturdayDelivery { get; set; }
        public bool containsAlcohol { get; set; }
        public bool mergedOrSplit { get; set; }
        public List<object> mergedIds { get; set; }
        public string parentId { get; set; }
        public string storeId { get; set; }
        public string customField1 { get; set; }
        public string customField2 { get; set; }
        public string customField3 { get; set; }
        public string source { get; set; }
        public string billToParty { get; set; }
        public string billToAccount { get; set; }
        public string billToPostalCode { get; set; }
        public string billToCountryCode { get; set; }
    }
}
