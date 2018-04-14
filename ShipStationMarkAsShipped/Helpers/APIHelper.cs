using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipStationMarkAsShipped.Helpers
{
    public static class APIHelper
    {
        public static string GetApiKey()
        {
           
            return ("abc");
        }

        public static Uri GetBaseUri()
        {
            return (new Uri("https://ssapi.shipstation.com/"));
        }

        public static string GetSharedSecretKey()
        {
            return ("def");
        }

        public static string GetShipstationUserID()
        {
            return ("ghi");
        }

    }
}
