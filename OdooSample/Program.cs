using Microsoft.Extensions.Configuration;
using Odoo.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OdooSample
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var rpcConnnectionSettings = new RpcConnectionSetting();
            config.GetSection("OdooConnection").Bind(rpcConnnectionSettings);

            var odooConn = new RpcConnection(rpcConnnectionSettings);

            var partner_1 = new RpcModel("res.partner", odooConn);
           
            var method_response = partner_1.CallMethod("find_or_create", new object[1] { "ssssssssss@gddd.com"});


            //res.partner - Write
            var partner = new RpcContext(odooConn, "res.partner");

            partner.AddFields(new[] { "name", "phone", "email" });

            var results = partner.Execute(false, order: "id desc");
            //var results = partner.Execute(false, offset:1, limit:2, order: "id desc");
            foreach (var result in results)
            {
                result.SetFieldValue("phone", "55-66-666");
                result.Save();
            }

            //res.partner - Create
            RpcRecord record = new RpcRecord(odooConn, "stock.quant", -1, new List<RpcField>
            {
                new RpcField{FieldName = "product_id"},
                new RpcField{FieldName = "reserved_quantity"},
                new RpcField{FieldName = "location_id"}
            });
            record.SetFieldValue("product_id", 52);
            record.SetFieldValue("reserved_quantity", 333);
            record.SetFieldValue("location_id", 8);
            record.Save();
        }
    }
}
