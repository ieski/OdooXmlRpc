using Microsoft.Extensions.Configuration;
using Odoo.Concrete;
using System;
using System.Collections.Generic;

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
            var rpcContext = new RpcContext(odooConn, "res.partner");

            
            //Query Parameter
            rpcContext
                .RpcFilter
                .Or()
                .Equal("id", 1)
                .Equal("id", 14383);

            
            //Returns all fields if fields are not selected
            rpcContext
                .AddField("id")
                .AddField("name")
                .AddField("mobile")
                .AddField("phone");

            var data = rpcContext.Execute(true, limit:5);

            foreach (var record in data)
            {
                var id = record.GetField("id");
                var  name = record.GetField("name");
                var  mobile = record.GetField("mobile");
                var  phone = record.GetField("phone");
                
                Console.WriteLine(record.GetField("id"));
                Console.WriteLine(record);
                
                // Update Record
                record.SetFieldValue("mobile","xxx-xxx-xx");
                record.Save();
            }

            
            //New Record
            var fields = new List<RpcField>
            {
                new RpcField {FieldName = "name", Value = 1},
                new RpcField {FieldName = "website", Value = "www.odootr.com"},
            };
            var newRecord = new RpcRecord(odooConn,"res.partner",null, fields);
            newRecord.Save();
            
            
            Console.ReadLine();
        }
    }
}
