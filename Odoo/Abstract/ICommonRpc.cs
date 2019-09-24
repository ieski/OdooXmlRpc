using System;
using CookComputing.XmlRpc;

namespace Odoo.Abstract
{
    [XmlRpcUrl("common")]
    public interface ICommonRpc : IXmlRpcProxy
    {
        [XmlRpcMethod("login")]
        int login(String dbName, string dbUser, string dbPwd);
    }
}
