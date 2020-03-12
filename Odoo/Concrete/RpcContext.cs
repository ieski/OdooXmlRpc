﻿using CookComputing.XmlRpc;
using Odoo.Model;
using System.Collections;
using System.Collections.Generic;

namespace Odoo.Concrete
{
    public class RpcContext
    {
        protected readonly RpcConnection _rpcConnection;
        private readonly string _modelName;
        private RpcModel _rpcModel;
        private List<RpcRecord> _records;

        public List<string> FieldNames { get; set; }
        public RpcFilter RpcFilter { get; set; }

        public RpcContext(RpcConnection rpcConnection, string modelName)
        {
            _rpcConnection = rpcConnection;
            _modelName = modelName;
            RpcFilter = new RpcFilter();
            FieldNames = new List<string>();
            _rpcModel = new RpcModel(_modelName, _rpcConnection);
            _records = new List<RpcRecord>();
        }

        public string ModelName => _modelName;
        public List<RpcRecord> Records => _records;

        public RpcConnection Connection => _rpcConnection;
        public List<FieldAttribute> GetFields()
        {
            if (!_rpcConnection.Login())
            {
                _rpcConnection.Login();
            }

            var result = (XmlRpcStruct)_rpcModel.GetFields();

            var fields = new List<FieldAttribute>();

            foreach (DictionaryEntry entry in result)
            {
                var fieldAttribute = (XmlRpcStruct) entry.Value;

                fields.Add(new FieldAttribute
                {
                    FieldName = entry.Key.ToString(),
                    Type = fieldAttribute.ContainsKey("type") ? fieldAttribute["type"].ToString() : "",
                    Help = fieldAttribute.ContainsKey("help") ? fieldAttribute["help"].ToString() : "",
                    String = fieldAttribute.ContainsKey("string") ? fieldAttribute["string"].ToString() : ""
                });
            }

            return fields;
        }

        /// <summary>
        ///     Read parametresini True atarsan ODOO üzerinde READ işleminide yapar
        /// </summary>
        /// <param name="read"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<RpcRecord> Execute(bool read = false, int offset = 0, int? limit = null)
        {
            if (!_rpcConnection.Login()) _rpcConnection.Login();

            _rpcModel.AddFields(FieldNames);

            _records = !read? _rpcModel.Search(RpcFilter.ToArray()) : _rpcModel.SearchAndRead(RpcFilter.ToArray(), offset, limit);
            return _records;
        }

        /// <summary>
        /// Toplam kayıt sayısı
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            if (!_rpcConnection.Login()) _rpcConnection.Login();

            var filter = RpcFilter.ToArray();

            return _rpcModel.Count(filter);
        }

      
        public RpcContext AddField(string fieldName)
        {
            FieldNames.Add(fieldName);
            return this;
        }

        public RpcContext AddFields(List<string> fieldsName)
        {
            FieldNames.AddRange(fieldsName);
            return this;
        }

    }
}
