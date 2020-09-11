using Odoo.Concrete;
using System.Xml.Linq;

namespace Odoo.Extensions
{
    public static class RpcContextExtesionXml
    {
        public static XElement ToXml(this RpcContext context)
        {
            var root = new XElement("records");

            foreach (var record in context.GetRecords())
            {
                var element = new XElement("Record");
                foreach (var field in record.GetFields())
                {
                    if (field.Value == null) field.Value = "";                      // 2020-09-08 added by Marc Trudel 
                    element.Add(new XAttribute(field.FieldName,  field.Value));
                }
                root.Add(element);
            }
            return root;
        }
    }
}
