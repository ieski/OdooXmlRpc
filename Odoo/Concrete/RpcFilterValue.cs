using System.Collections;

namespace Odoo.Concrete
{
    public class RpcFilterValue : ArrayList
    {
        public RpcFilterValue AddValue(object value)
        {
            Add(value);
            return this;
        }
    }
}