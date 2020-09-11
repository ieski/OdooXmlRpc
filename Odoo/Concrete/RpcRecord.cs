using CookComputing.XmlRpc;
using System.Collections.Generic;
using System.Linq;

namespace Odoo.Concrete
{
    public class RpcRecord
    {
        private readonly RpcConnection _rpcConnection;
        private readonly string _model;
        private readonly List<RpcField> _fieldsResult;
        private int _id = -1;

        public int Id => _id;

        public RpcRecord(RpcConnection rpcConnection, string model, int? id, IEnumerable<RpcField> fieldsTemplate,
            XmlRpcStruct vals = null)
        {
            _model = model;
            _rpcConnection = rpcConnection;
            if (id == null)
            {
                _id = -1;
            }
            else
            {
                _id = (int)id;
            }

            if (id != null)
            {
                _fieldsResult = new List<RpcField>();
                if (fieldsTemplate == null) return;
                foreach (var rpcField in fieldsTemplate)
                {
                    _fieldsResult.Add(new RpcField
                    {
                        FieldName = rpcField.FieldName,
                        Type = rpcField.Type,
                        String = rpcField.String,
                        Help = rpcField.Help,
                        Changed = false,
                        Value = vals?[rpcField.FieldName]
                    });
                }

            }
            else
            {
                _fieldsResult = fieldsTemplate.ToList();
            }
        }

        public IEnumerable<RpcField> GetFields()
        {
            return _fieldsResult;
        }

        public void SetFieldValue(string field, object value)
        {
            var fieldAttribute = _fieldsResult.FirstOrDefault(f => f.FieldName == field);
            if (fieldAttribute == null) return;

            fieldAttribute.Changed = fieldAttribute.Changed == false;

            fieldAttribute.Value = value;
        }

        public RpcField GetField(string field, object defaultValue = null)
        {
            var fieldAttribute = _fieldsResult.FirstOrDefault(f => f.FieldName == field);
            if (fieldAttribute != null) fieldAttribute.DefaultValue = defaultValue;
            return fieldAttribute;
        }

        public void Save()
        {
            var values = new XmlRpcStruct();

            if (_id >= 0)
            {
                foreach (var field in _fieldsResult.Where(f => (bool)f.Changed))
                {
                    values[field.FieldName] = field.Value;
                }

                _rpcConnection.Write(_model, new int[1] { _id }, values);
            }
            else
            {
                foreach (var field in _fieldsResult)
                {
                    values[field.FieldName] = field.Value;
                }

                _id = _rpcConnection.Create(_model, values);
            }
        }

        public override string ToString()
        {
            var value = "";
            foreach (var field in _fieldsResult)
            {
                value += $"{field.FieldName}: {field.Value} \n";
            }

            return value;
        }

        // 2020-09-08 added by Marc Trudel 
        public string ToCsvString()

        {
            var value = "";
            foreach (var field in _fieldsResult)
            {
                value += $"{field.Value},";
            }
            value = value.Substring(0, value.Length - 1);     // remove last ,
            return value;
        }
    }
}