﻿using CookComputing.XmlRpc;
 using System.Collections;
using System.Collections.Generic;
 using System.Linq;

 namespace Odoo.Concrete
{
    public class RpcContext
    {
        private readonly RpcConnection _rpcConnection;
        private readonly RpcModel _rpcModel;
        private List<string> _fieldNames;
        
        public RpcFilter RpcFilter { get;}
        private List<RpcRecord> _records { get; set; }

        public RpcContext(RpcConnection rpcConnection, string modelName)
        {
            _rpcConnection = rpcConnection;
            _rpcModel = new RpcModel(modelName, _rpcConnection);
            
            _records = new List<RpcRecord>();
            RpcFilter = new RpcFilter();
            _fieldNames = new List<string>();
        }

        private List<RpcField> GetFields()
        {
            if (!_rpcConnection.Login()) _rpcConnection.Login();

            object[] filter = _fieldNames.ToArray();

            var result = (XmlRpcStruct)_rpcModel.GetFields(filter);

            var fields = new List<RpcField>();

            foreach (DictionaryEntry entry in result)
            {
                var fieldAttribute = (XmlRpcStruct) entry.Value;

                fields.Add(new RpcField
                {
                    FieldName = entry.Key.ToString(),
                    Type = fieldAttribute.ContainsKey("type") ? fieldAttribute["type"].ToString() : "",
                    Help = fieldAttribute.ContainsKey("help") ? fieldAttribute["help"].ToString() : "",
                    String = fieldAttribute.ContainsKey("string") ? fieldAttribute["string"].ToString() : ""
                });
            }

            return fields;
        }

        public IEnumerable<RpcRecord> Execute(bool read = false, int offset = 0, int? limit = null)
        {
            if (!_rpcConnection.Login()) _rpcConnection.Login();

            var fieldsResult = GetFields();
            
            if (_fieldNames.Count == 0)
            {
                _fieldNames = fieldsResult.Select(f => f.FieldName).ToList(); 
            }
            
            _rpcModel.AddFields(_fieldNames);

            return _records = !read? _rpcModel.Search(RpcFilter.ToArray()) : _rpcModel.SearchAndRead(RpcFilter.ToArray(), fieldsResult, offset, limit);
        }

        public List<RpcRecord> GetRecords()
        {
            return _records;
        }
        
        /// <summary>
        /// Total record count
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
            _fieldNames.Add(fieldName);
            return this;
        }

        public RpcContext AddFields(IEnumerable<string> fieldsName)
        {
            _fieldNames.AddRange(fieldsName);
            return this;
        }

    }
}
