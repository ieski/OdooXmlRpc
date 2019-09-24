using Microsoft.Extensions.Configuration;
using Odoo.Concrete;
using System;

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
            var rpcContext = new RpcContext(odooConn, "res.partner");

            rpcContext.RpcFilter
                .Equal("id", 10);

            rpcContext
                .AddField("id")
                .AddField("name");

            var data = rpcContext.Execute(true);


            Console.ReadLine();

        }
    }
}
