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
