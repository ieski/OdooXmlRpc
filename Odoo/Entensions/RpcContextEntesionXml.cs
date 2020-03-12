using Odoo.Concrete;
using System.Xml.Linq;

namespace Odoo.Entensions
{
    public static class RpcContextEntesionXml
    {
        public static XElement ToXml(this RpcContext context)
        {
            var root = new XElement(context.ModelName);

            foreach (var record in context.Records)
            {
                var element = new XElement("Item");
                foreach (var field in record.GetFields())
                {
                    element.Add(new XAttribute(field.Key, field.Value));
                }
                root.Add(element);
            }
            return root;
        }
    }
}
