using Microsoft.Extensions.Configuration;
using Odoo.Concrete;
using System;
using System.Linq;
using System.Reflection.Metadata;


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
                .RpcFilter.Equal("id", 1458);

            var fields = rpcContext.GetFields();
            foreach (var field in fields)
            {
                rpcContext.AddField(field.FieldName);
            }
            rpcContext.AddFields(fields.Select(f => f.FieldName).ToList());
            
            rpcContext
                .AddField("id")
                .AddField("image_medium");

            var data = rpcContext.Execute(true, limit:5);

            foreach (var record in data.ToList())
            {
                var image = "";
                if (record.GetValue("image_medium") != null)
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
