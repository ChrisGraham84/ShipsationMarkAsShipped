using ShipStationMarkAsShipped.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShipStationMarkAsShipped.Helpers
{
    public static class ExportHelpers
    {
        public static void ExtractionReport(List<Order> Orders, ShipstationCustomer Company, List<Store> Stores)
        {
            List<ExtractionModel> extractions = new List<ExtractionModel>();
            var carriers = ImportHelper.GetObjectFromJsonArray<Carrier>("carriers.js");
            var services = ImportHelper.GetObjectFromJsonArray<Service>("services.js");

            foreach (var order in Orders)
            {
                var store = Stores.Where(x => x.storeId == int.Parse(order.advancedOptions.storeId)).FirstOrDefault();
                var carrier = carriers.Where(x => x.code == order.carrierCode).FirstOrDefault();
                var service = services.Where(x => x.code == order.serviceCode).FirstOrDefault();
                foreach (var item in order.items)
                {
                    ExtractionModel em = new ExtractionModel();
                    em.etsy_company_id = Company.CompanyId;
                    if(carrier != null)
                    {
                        em.Carrier_Carrier_Selected = carrier.name;
                        em.Carrier_Service_Selected = service.name;
                    }
                    else
                    {
                        em.Carrier_Carrier_Selected = "";
                        em.Carrier_Service_Selected = "";
                    }
                    em.Carrier_Service_Requested = order.requestedShippingService;

                    if(order.advancedOptions != null)
                    {
                        em.Custom_Field_1 = order.advancedOptions.customField1;
                        em.Custom_Field_2 = order.advancedOptions.customField2;
                        em.Custom_Field_3 = order.advancedOptions.customField3;
                    }
                    else
                    {
                        em.Custom_Field_1 = "";
                        em.Custom_Field_2 = "";
                        em.Custom_Field_3 = "";
                    }
                   
                    em.Date_Order_Date = order.orderDate;
                    em.Gift_Message = (order.giftMessage != null ? order.giftMessage.Replace("\t"," ").Replace("\n"," ") : "");
                    if(order.insuranceOptions.insuredValue == "0")
                    {
                        em.Insurance_Cost = "0.00";
                    }
                    else
                    {
                        em.Insurance_Cost = order.insuranceOptions.insuredValue;
                    }
                    em.Item_Fill_SKU = item.fulfillmentSku;
                    em.Item_Image_URL = item.imageUrl;
                    em.Item_Name = item.name;
                    em.Item_ISBN = item.upc;

                    if (store.marketplaceName == "Etsy")
                    {
                        if (item.options.Count > 0)
                        {
                            em.Item_Options = string.Format("{0}: {1}, {2}: {3}", item.options[0].name, item.options[0].value, item.options[1].name, item.options[1].value);
                            em.Variation_Value = string.Format("{0},{1}",item.options[0].value.Replace("&#39;", "'"), item.options[1].value.Replace("&#39;", "'"));
                        }
                        else
                        {
                            em.Item_Options = "";
                        }
                    }
                    else if(store.marketplaceName == "Amazon")
                    {
                        em.Item_Options = item.sku;
                    }
                    else
                    {
                        if (item.options.Count > 0)
                        {
                            em.Item_Options = string.Format("{0}: {1}, {2}: {3}", item.options[0].name, item.options[0].value, item.options[1].name, item.options[1].value);
                        }
                        else
                        {
                            em.Item_Options = "";
                        }
                    }
                    em.Item_Qty = item.quantity;
                    em.Item_SKU = item.sku;
                    em.Market_Markeplace_Name = store.marketplaceName;
                    em.Market_Market_Order_URL = "";
                    em.Market_Store_Name = store.storeName;
                    em.Notes_From_Buyer = (order.customerNotes != null ? order.customerNotes.Replace("\t", " ").Replace("\n", " ").Replace("\r", "") : "");
                    em.Notes_Internal = (order.internalNotes != null ? order.internalNotes.Replace("\t", " ").Replace("\n", " ").Replace("\r", "") : "");
                    em.Order_Number = order.orderNumber;
                    em.Order_Status = order.orderStatus;
                    em.Ship_To_Address_1 = order.shipTo.street1;
                    em.Ship_To_Address_2 = order.shipTo.street2;
                    em.Ship_To_Address_3 = order.shipTo.street3;
                    em.Ship_To_City = order.shipTo.city;
                    em.Ship_To_Company = order.shipTo.company;
                    em.Ship_To_Country = order.shipTo.country;
                    em.Ship_To_FirstName = FormattingHelpers.FirstNameLastNameFormatter(order.shipTo.name).Item1;
                    em.Ship_To_LastName = FormattingHelpers.FirstNameLastNameFormatter(order.shipTo.name).Item2;
                    em.Ship_To_Phone = order.shipTo.phone;
                    em.Ship_To_Postal_Code = order.shipTo.postalCode;
                    em.Ship_To_State = order.shipTo.state;

                    extractions.Add(em);
                }

            }

            List<string> properties = new ExtractionModel().GetType().GetProperties().Select(x => x.Name).ToList();

            StringBuilder sbHeader = new StringBuilder();
            foreach (var prop in properties)
            {
                sbHeader.AppendFormat("{0}\t", prop);
            }

            using (StreamWriter writer = new StreamWriter(System.Configuration.ConfigurationManager.AppSettings["Path"] + Company.CompanyName + " orders.txt"))
            {
                
                writer.WriteLine(sbHeader.ToString());
                foreach (var ext in extractions.OrderBy(x=>x.Order_Number))
                {
                    StringBuilder sbData = new StringBuilder();
                    foreach (var prop in properties)
                    {
                        var o = ext.GetType().GetProperty(prop).GetValue(ext, null);
                        if (o == null)
                        {
                            o = "";
                        }
                        sbData.AppendFormat("{0}\t", o.ToString());
                    }
                    writer.WriteLine(sbData.ToString());
                }
            }
        }
    }
}
