OdooXmlRpc .Net
Description
XmlRpc Web Service Client .NET is a C# implementation of XML-RPC, a popular protocol that uses XML over HTTP to implement remote procedure calls. This implementation can be used in .NET 4.6 This software was tested with Odoo ERP 8 and 11

Features
Copyright: 2019 İsmail Eski ismaileski@gmail.com
Repository: https://github.com/ieski/OdooXmlRpc
License: LGPL 3
Language: C#, .NET 4.6
IDE: Visual Studio 2019
Version: v1.0.0

Links
http://xmlrpc.scripting.com/
https://en.wikipedia.org/wiki/XML-RPC

Example Query HR.EMPLOYEE

Source:
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
                if ((bool) record.GetValue("image_medium"))
                {
                  var image = record.GetValue("image_medium").ToString();
                }
                
                if (image.Length <= 0) continue;
                var id = record.GetValue("id").ToString();
            }
            Console.ReadLine();
        }
    }
}
