using System.Collections;
using System.Collections.Generic;

namespace Odoo.Concrete
{
    public class RpcFilter : ArrayList
    {
        public RpcFilter Equal(string fieldName, object value)
        {
            var field = new object[] { fieldName, "=", value };
            Add(field);
            return this;
        }

        public RpcFilter Or()
        {
            Add("|");
            return this;
        }

        public RpcFilter Not()
        {
            Add("!");
            return this;
        }

        public RpcFilter And()
        {
            Add("&");
            return this;
        }

        public RpcFilter ILike(string fieldName, object value)
        {
            var field = new object[] { fieldName, "ilike", value };
            Add(field);
            return this;
        }

        public RpcFilter Like(string fieldName, object value)
        {
            var field = new object[] { fieldName, "like", value };
            Add(field);
            return this;
        }

        public RpcFilter NotLike(string fieldName, object value)
        {
            var field = new object[] { fieldName, "not like", value };
            Add(field);
            return this;
        }

        public RpcFilter NotEqual(string fieldName, object value)
        {
            var field = new object[] { fieldName, "!=", value };
            Add(field);
            return this;
        }

        public RpcFilter GreaterThan(string fieldName, object value)
        {
            var field = new object[] { fieldName, ">", value };
            Add(field);
            return this;
        }

        public RpcFilter GreaterThanOrEqual(string fieldName, object value)
        {
            var field = new object[] { fieldName, ">=", value };
            Add(field);
            return this;
        }

        public RpcFilter LessThan(string fieldName, object value)
        {
            var field = new object[] { fieldName, "<", value };
            Add(field);
            return this;
        }

        public RpcFilter LessThanOrEqual(string fieldName, object value)
        {
            var field = new object[] { fieldName, "<=", value };
            Add(field);
            return this;
        }

        public RpcFilter Between(string fieldName, object value1, object value2)
        {
            var field = new object[] { fieldName, "between", value1, "and", value2 };
            Add(field);
            return this;
        }

        public RpcFilter In(string fieldName, RpcFilterValue value)
        {
            var field = new object[] { fieldName, "in", value.ToArray() };
            Add(field);
            return this;
        }

        public RpcFilter In(string fieldName, object[] values)
        {
            var field = new object[] { fieldName, "in", values };
            Add(field);
            return this;
        }

        public RpcFilter In(string fieldName, List<object> values)
        {
            var field = new object[] { fieldName, "in", values.ToArray() };
            Add(field);
            return this;
        }

        public RpcFilter In(string fieldName, int[] values)
        {
            var field = new object[] { fieldName, "in", values };
            Add(field);
            return this;
        }
        
        public RpcFilter NotIn(string fieldName, RpcFilterValue value)
        {
            var field = new object[] { fieldName, "not in", value.ToArray() };
            Add(field);
            return this;
        }

        public RpcFilter NotIn(string fieldName, object[] values)
        {
            var field = new object[] { fieldName, "not in", values };
            Add(field);
            return this;
        }

        public RpcFilter NotIn(string fieldName, List<object> values)
        {
            var field = new object[] { fieldName, "not in", values.ToArray() };
            Add(field);
            return this;
        }

        public RpcFilter NotIn(string fieldName, int[] values)
        {
            var field = new object[] { fieldName, "not in", values };
            Add(field);
            return this;
        }

        public RpcFilter ChildOf(string fieldName, string values)
        {
            var field = new object[] { fieldName, "child_of", values };
            Add(field);
            return this;
        }
    }
}