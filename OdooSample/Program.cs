using Microsoft.Extensions.Configuration;
using Odoo.Concrete;
using System;
using System.Linq;


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
                .RpcFilter.NotEqual("x_emp_uniqueid", false);

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
