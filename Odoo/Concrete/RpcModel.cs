﻿using System.Collections.Generic;
using System.Linq;
using CookComputing.XmlRpc;

namespace Odoo.Concrete
{
    public class RpcModel
    {
        private readonly string _modelName;
        private readonly RpcConnection _rpcConnection;
        private readonly List<string> _fields;

        public RpcModel(string modelName, RpcConnection rpcConnection)
        {
            _rpcConnection = rpcConnection;
            _modelName = modelName;
            _fields = new List<string>();
        }

        public int Count(object[] filter)
        {
            int count = _rpcConnection.Count(_modelName, filter);
            return count;
        }

        public object GetFields()
        {
            return _rpcConnection.GetFields(_modelName, new object[] { "string", "help", "type" });
        }

        public List<RpcRecord> SearchAndRead(object[] filter, int offset = 0, int? limit = null)
        {
            var records = new List<RpcRecord>();

            object[] result = _rpcConnection.SearchAndRead(_modelName, filter, _fields.ToArray(), offset, limit);

            foreach (object entry in result)
            {
                var vals = (XmlRpcStruct)entry;

                // Get ID
                int id = (int)vals["id"];
                var record = new RpcRecord(_rpcConnection, _modelName, id);

                // Get other values
                foreach (var field in _fields.Where(field => vals.ContainsKey(field)))
                {
                    record.SetValue(field, vals[field]);
                }
                records.Add(record);
            }

            return records;
        }

        public List<RpcRecord> Search(object[] filter)
        {
            var ids = _rpcConnection.Search(_modelName, filter);

            return ids.Select(id => new RpcRecord(_rpcConnection, _modelName, id)).ToList();
        }

        public void AddField(string field)
        {
            if (!_fields.Contains(field))
            {
                _fields.Add(field);
            }
        }

        public void AddFields(List<string> fields)
        {
            foreach (var field in fields)
            {
                AddField(field);
            }
        }

        public void Remove(List<RpcRecord> records)
        {
            var toRemove = records
                                    .Where(r => r.Id >= 0)
                                    .Select(r => r.Id)
                                    .ToArray();

            _rpcConnection.Remove(_modelName, toRemove);
        }

        public void Remove(RpcRecord rpcRecord)
        {
            Remove(new List<RpcRecord>() { rpcRecord });
        }

        public void Save(List<RpcRecord> records)
        {
            foreach (var record in records)
            {
                record.Save();
            }
        }

        public void Save(RpcRecord rpcRecord)
        {
            Save(new List<RpcRecord>() { rpcRecord });
        }

        public RpcRecord CreateNew()
        {
            return new RpcRecord(_rpcConnection, _modelName, -1);
        }
    }
}
