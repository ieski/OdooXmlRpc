namespace Odoo.Concrete
{
    public class RpcField
    {
        public string FieldName { get; set; }
        public string Type { get; set; }
        public string Help { get; set; }
        public string String { get; set; }
        public object DefaultValue { get; set; } = null;

        private object _value;

        public object Value
        {
            get { 
                if (Type != "bool" && Type != "boolean" && _value is bool && ((bool)_value) == false) 
                { 
                    return DefaultValue; 
                } 
                return _value; 
            }
            
            set
            {
                _value = value;
            }
        }

        public bool? Changed { get; set; }

        public override string ToString()
        {
            return $"{FieldName} - {Value} - {Type} - {String} - {Help}";
        }
    }
}
