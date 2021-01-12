using System;
using Microsoft.Extensions.Configuration;
using Odoo.Concrete;
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

            //Connection
            var odooConn = new RpcConnection(rpcConnnectionSettings);

            //SaleOrder
            WriteSaleOrder(odooConn);

        }

        static RpcRecord WriteSaleOrder(RpcConnection conn)
        {
            //Partner
            var partner = WritePartner(conn);
            var rnd = new Random();

            var orderline = OrderLine(conn);

            //Sale Order - Create
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


        static List<object> OrderLine(RpcConnection conn)
        {
            var orderLine = new List<object>();

            //Ürün
            var product = GetProduct(conn, "11.BK.076.01");

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

            return orderLine;
        }

        static RpcRecord GetProduct(RpcConnection conn, string defaultCode)
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

        static RpcRecord WritePartner(RpcConnection conn)
        {
            //İl
            var stateId = GetCountryState(conn, "İstanbul");

            //res.partner - Create
            RpcRecord record = new RpcRecord(conn, "res.partner", -1, new List<RpcField>
            {
                new RpcField{FieldName = "name", Value = "Ayhan Akbaş"},
                new RpcField{FieldName = "street", Value = "Merkez"},
                new RpcField{FieldName = "street2", Value = "Merkez Mh."},
                new RpcField{FieldName = "state_id", Value = stateId},
                new RpcField{FieldName = "vat", Value = "TR1234567890"}
            });
            record.Save();
            return record;
        }


        static int GetCountryState(RpcConnection conn, string stateName)
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
}
