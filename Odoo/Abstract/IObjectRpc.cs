using System;
using System.Collections.Generic;
using CookComputing.XmlRpc;

namespace Odoo.Abstract
{
    [XmlRpcUrl("object")]
    public interface IObjectRpc : IXmlRpcProxy
    {
        [XmlRpcMethod("execute")]
        int create(String dbName, int userId, string dbPwd, string model, string method, XmlRpcStruct fieldValues);

        [XmlRpcMethod("execute")]
        int[] search(string dbName, int userId, string dbPwd, string model, string method, object[] filter, int offset, int limit, string order);

        [XmlRpcMethod("execute_kw")]
        object[] search_read(string dbName, int userId, string dbPwd, string model, string method, object[] filter, object[] fields, int offset, int limit, string order);

        [XmlRpcMethod("execute")]
        object fields_get(string dbName, int userId, string dbPwd, string model, string method, object[] filter, object[] attributes);

        [XmlRpcMethod("execute")]
        int search_count(string dbName, int userId, string dbPwd, string model, string method, object[] filter);

        [XmlRpcMethod("execute")]
        object[] read(string dbName, int userId, string dbPwd, string model, string method, int[] ids, object[] fields);

        [XmlRpcMethod("execute")]
        bool write(string dbName, int userId, string dbPwd, string model, string method, int[] ids, XmlRpcStruct fieldValues);

        [XmlRpcMethod("execute")]
        bool unlink(string dbName, int userId, string dbPwd, string model, string method, int[] ids);

        [XmlRpcMethod("exec_workflow")]
        bool exec_workflow(string dbName, int userId, string dbPwd, string model, string action, int ids);

        [XmlRpcMethod("render_report")]
        string render_report(string dbName, int userId, string dbPwd, string report, int ids);

        [XmlRpcMethod("execute_kw")]
        object call(string dbName, int userId, string dbPwd, string model, string method, object[] parameters);
        
        [XmlRpcMethod("execute_kw")]
        object call(string dbName, int userId, string dbPwd, string model, string method, object[] parameters, object[] parameters_kw);
    }
}
