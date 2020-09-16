using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MapperAttribute:Attribute
    {
        /// <summary>
        /// the Property Name that map to
        /// </summary>
        public string MapTo { get; set; }

        public MapperAttribute() { }
        public MapperAttribute(string mapTo)
        {
            this.MapTo = mapTo;
        }

        public static string GetMapToPropertyName(PropertyInfo propertyInfo)
        {
            object[] mapperAttrs = propertyInfo.GetCustomAttributes(typeof(MapperAttribute), false);

            MapperAttribute mapperAttr;

            if (mapperAttrs != null && mapperAttrs.Count() >= 1)
            {
                mapperAttr = mapperAttrs[0] as MapperAttribute;

                return mapperAttr.MapTo;
            }
            else
            {
                return propertyInfo.Name;
            }
        }
    }
}
