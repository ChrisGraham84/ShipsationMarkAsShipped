using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipStationMarkAsShipped.Model
{
    public class Carrier
    {
        public string name { get; set; }
        public string code { get; set; }
        public string accountNumber { get; set; }
        public bool requiresFundedAccount { get; set; }
        public string balance { get; set; }
    }

    public class Service
    {
        public string carrierCode { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public bool domestic { get; set; }
        public bool international { get; set; }
    }
}
