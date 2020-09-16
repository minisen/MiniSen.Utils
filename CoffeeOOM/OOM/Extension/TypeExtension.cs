using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOM.Extension
{
    public static class TypeExtension
    {
        /// <summary>
        /// Get All level inherit class types of current type
        /// </summary>
        /// <param name="currentType"></param>
        /// <param name="outInheritClassList">storage result of type</param>
        public static void GetInheritClassTypes(Type currentType, ref List<Type> outInheritClassList)
        {
            outInheritClassList.Add(currentType);

            if (currentType.BaseType.Name != "Object")
            {
                GetInheritClassTypes(currentType.BaseType, ref outInheritClassList);
            }
            else
            {
                return;
            }
        }
    }
}
