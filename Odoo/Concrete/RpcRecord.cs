using System.Collections.Generic;
using CookComputing.XmlRpc;

namespace Odoo.Concrete
{
    public class RpcRecord
    {
        private readonly RpcConnection _rpcConnection;
        private readonly string _model;
        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();
        private readonly List<string> _modifiedFields = new List<string>();
        int _id = -1;

        public RpcRecord(RpcConnection rpcConnection, string model, int id)
        {
            _model = model;
            _rpcConnection = rpcConnection;
            _id = id;
        }

        public Dictionary<string, object> GetFields()
        {
            return _fields;
        }

        public bool SetValue(string field, object value)
        {
            if (_fields.ContainsKey(field))
            {
                if (!_modifiedFields.Contains(field))
                {
                    _modifiedFields.Add(field);
                }

                _fields[field] = value;
            }
            else
            {
                _fields.Add(field, value);
            }
            return true;
        }

        public object GetValue(string field)
        {
            if (!_fields.ContainsKey(field)) return null;
            if (_fields[field] is bool && !(bool)_fields[field]) return null;

            return _fields[field];
        }

        public int Id => _id;

        public void Save()
        {
            var values = new XmlRpcStruct();

            if (_id >= 0)
            {
                foreach (var field in _modifiedFields)
                {
                    values[field] = _fields[field];
                }

                _rpcConnection.Write(_model, new int[1] { _id }, values);
            }
            else
            {
                foreach (var field in _fields.Keys)
                {
                    values[field] = _fields[field];
                }

                _id = _rpcConnection.Create(_model, values);
            }
        }
    }
}
