using System;
using System.ComponentModel;
using System.Linq;

namespace CM50_Configurator.Support
{
    // La PropertyGrid ordina le proprietà alfabeticamente per default. Qui l'ordine
    // deve invece rispecchiare quello delle colonne del database, indicato con [PropertyOrder].
    public class OrderedExpandableObjectConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = base.GetProperties(context, value, attributes);

            PropertyDescriptor[] ordered = properties
                .Cast<PropertyDescriptor>()
                .OrderBy(GetOrder)
                .ToArray();

            return new PropertyDescriptorCollection(ordered);
        }

        private static int GetOrder(PropertyDescriptor property)
        {
            var attribute = property.Attributes[typeof(PropertyOrderAttribute)] as PropertyOrderAttribute;
            return attribute?.Order ?? int.MaxValue;
        }
    }
}
