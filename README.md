OdooXmlRpc .Net
================

Description
-----------
XmlRpc Web Service Client .NET is a C# implementation of XML-RPC, a popular protocol that uses XML over HTTP to implement remote procedure calls. This implementation can be used in .NET 4.6 This software was tested with Odoo ERP 8 and 11

Features
--------
- Copyright: 2019 İsmail Eski ismaileski@gmail.com
- Repository: https://github.com/ieski/OdooXmlRpc
- License: LGPL 3
- Language: C#, .NET 4.6
- IDE: Visual Studio 2019
- Version: v1.0.0

Links
-----
- http://xmlrpc.scripting.com/
- https://en.wikipedia.org/wiki/XML-RPC


Example Create Sale Order And Create Partner
-------------------------
- Source:
```cs
	class Program
    {
        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var rpcConnnectionSettings = new RpcConnectionSetting();
            config.GetSection("OdooConnection").Bind(rpcConnnectionSettings);

            //Connection
            var odooConn = new RpcConnection(rpcConnnectionSettings);

            //SaleOrder
            WriteSaleOrder(odooConn);

        }

        //Create Sale Order
        static RpcRecord WriteSaleOrder(RpcConnection conn)
        {
            //Partner
            var partner = CreatePartner(conn);
            var rnd = new Random();

            var orderline = GetSaleOrderLine(conn);

            
            RpcRecord record = new RpcRecord(conn, "sale.order", -1, new List<RpcField>
            {
                new RpcField{FieldName = "company_id", Value = 1},
                new RpcField{FieldName = "currency_id", Value = 31},
                new RpcField{FieldName = "date_order", Value = "2021-01-12"},
                new RpcField{FieldName = "name", Value = "Örnek Sipariş No:" + rnd.Next(1,10000).ToString()},
                new RpcField{FieldName = "partner_id", Value = partner.Id},
                new RpcField{FieldName = "partner_invoice_id", Value = partner.Id},
                new RpcField{FieldName = "partner_shipping_id", Value = partner.Id},
                new RpcField{FieldName = "picking_policy", Value = "one"},
                new RpcField{FieldName = "pricelist_id", Value = 1},
                new RpcField{FieldName = "warehouse_id", Value = 5},
                new RpcField{FieldName = "state", Value = "sale"}, //Onaylı Sipariş ise
                new RpcField{FieldName = "order_line", Value =  orderline.ToArray() }
            });

            record.Save();
            return record;
        }


        //Sale Order Satırlarını Oluştur
        static List<object> GetSaleOrderLine(RpcConnection conn)
        {
            var orderLine = new List<object>();

            //Add Product 1
            var product = GetSearchProductByDefaultCode(conn, "10.RF.091.00");

            RpcRecord record = new RpcRecord(conn, "sale.order.line", -1, new List<RpcField>
            {
                new RpcField{FieldName = "name", Value = product.GetField("name").Value},
                new RpcField{FieldName = "customer_lead", Value = 8},
                new RpcField{FieldName = "price_unit", Value = 12.45},
                new RpcField{FieldName = "product_uom_qty", Value = 5},
                new RpcField{FieldName = "product_id", Value = product.Id},
                new RpcField{FieldName = "tax_id", Value = product.GetField("taxes_id").Value},
            });

            orderLine.Add(
                new object[] { 0, 0, record.GetRecord() }
                );



            //Add Product 2
            var product2 = GetSearchProductByDefaultCode(conn, "10.RF.085.00");

            RpcRecord record2 = new RpcRecord(conn, "sale.order.line", -1, new List<RpcField>
            {
                new RpcField{FieldName = "name", Value = product2.GetField("name").Value},
                new RpcField{FieldName = "customer_lead", Value = 8},
                new RpcField{FieldName = "price_unit", Value = 65.75},
                new RpcField{FieldName = "product_uom_qty", Value = 12},
                new RpcField{FieldName = "product_id", Value = product2.Id},
                new RpcField{FieldName = "tax_id", Value = product2.GetField("taxes_id").Value},
            });

            orderLine.Add(
                new object[] { 0, 0, record2.GetRecord() }
            );

            return orderLine;
        }

        //Search Product
        static RpcRecord GetSearchProductByDefaultCode(RpcConnection conn, string defaultCode)
        {
            var rpcContext = new RpcContext(conn, "product.product");

            rpcContext
                .RpcFilter.Equal("default_code", defaultCode);

            rpcContext
                .AddField("id")
                .AddField("name")
                .AddField("taxes_id");

            var data = rpcContext.Execute(true, limit: 1);
            return data.FirstOrDefault();
        }

        // Create Partner
        static RpcRecord CreatePartner(RpcConnection conn)
        {
            //İl
            var stateId = GetCountryStateByName(conn, "İstanbul");

            //res.partner - Create
            RpcRecord record = new RpcRecord(conn, "res.partner", -1, new List<RpcField>
            {
                new RpcField{FieldName = "name", Value = "İsmail Eski"},
                new RpcField{FieldName = "street", Value = "Merkez"},
                new RpcField{FieldName = "street2", Value = "Merkez Mh."},
                new RpcField{FieldName = "state_id", Value = stateId},
                new RpcField{FieldName = "vat", Value = "TR1234567890"}
            });
            record.Save();
            return record;
        }

        //Search Country State
        static int GetCountryStateByName(RpcConnection conn, string stateName)
        {
            var rpcContext = new RpcContext(conn, "res.country.state");

            rpcContext
                .RpcFilter.Equal("name", stateName);

            rpcContext
                .AddField("id");

            var data = rpcContext.Execute(limit: 1);
            return data.FirstOrDefault().Id;

        }

    }
```

Example Query HR.EMPLOYEE
-------------------------
- Source:

```cs
using Microsoft.Extensions.Configuration;
using Odoo.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using Odoo.Entensions;

namespace OdooSample
{
    class Program
    {
        private static readonly IServiceProvider ServiceProvider;

        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var rpcConnnectionSettings = new RpcConnectionSetting();
            config.GetSection("OdooConnection").Bind(rpcConnnectionSettings);

            var odooConn = new RpcConnection(rpcConnnectionSettings);


            var rpcContext = new RpcContext(odooConn, "hr.employee");

            rpcContext
                .RpcFilter.NotEqual("image_medium", false);

            rpcContext
                .AddField("id")
                .AddField("image_medium");              

            var data = rpcContext.Execute(true);


            foreach (var record in data.ToList())
            {                
                var image = "";
                if (!(bool) record.GetValue("image_medium"))
                {
                  image = record.GetValue("image_medium").ToString();
                }
                
                if (image.Length <= 0) continue;
                var id = record.GetValue("id").ToString();
            }
            Console.ReadLine();
        }
    }
}
```


Example Write and Create res.partner
-------------------------
- Source:

```cs
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

            //res.partner - Write
            var partner = new RpcContext(odooConn, "res.partner");

            partner.AddFields(new[] {"name", "phone", "email"});
            partner.RpcFilter.Equal("name", "ismail eski");
            var results = partner.Execute(false,  order:"id desc");
            //var results = partner.Execute(false, offset:1, limit:2, order: "id desc");
            foreach (var result in results)
            {
                result.SetFieldValue("phone", "55-66-666");
                result.Save();
            }


            //res.partner - Create
            RpcRecord record = new RpcRecord(odooConn, "res.partner", -1, new List<RpcField>
            {
                new RpcField{FieldName = "name"},
                new RpcField{FieldName = "phone"},
                new RpcField{FieldName = "email"}
            });
            record.SetFieldValue("name", "ismail eski");
            record.SetFieldValue("phone", "111-222-333");
            record.SetFieldValue("email", "ismaileski@gmail.com");
            record.Save();
        }
    }
}
```

Call Method
-------------------------
- Source:

```cs
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
        }
    }
}
```
