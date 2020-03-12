namespace Odoo.Concrete
{
    public class RpcField
    {
        public string FieldName { get; set; }
        public string Type { get; set; }
        public string Help { get; set; }
        public string String { get; set; }

        private object _value;

        public object Value
        {
            get
            {
                if (Type != "bool" && _value is bool && ((bool) _value) == false)
                {
                    return null;
                }

                return _value;
            }
            set { _value = value; }
        }

        public bool? Changed { get; set; }

        public override string ToString()
        {
            return $"{FieldName} - {Value} - {Type} - {String} - {Help}";
        }
    }
}