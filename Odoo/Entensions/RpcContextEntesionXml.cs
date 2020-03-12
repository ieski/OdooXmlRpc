using Odoo.Concrete;
using System.Xml.Linq;

namespace Odoo.Entensions
{
    public static class RpcContextEntesionXml
    {
        public static XElement ToXml(this RpcContext context)
        {
            var root = new XElement("records");

            foreach (var record in context.GetRecords())
            {
                var element = new XElement("Record");
                foreach (var field in record.GetFields())
                {
                    element.Add(new XAttribute("FieldName", field.FieldName));
                    element.Add(new XAttribute("Value", field.Value));
                    element.Add(new XAttribute("Type", field.Type));
                    element.Add(new XAttribute("String", field.String));
                    element.Add(new XAttribute("Help", field.Help));
                }
                root.Add(element);
            }
            return root;
        }
    }
}
