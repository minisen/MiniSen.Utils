using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OOM.Attributes;

namespace OOM
{
    public class EntityBase:IDisposable
    {
        /// <summary>
        /// storage QueryField's Result
        /// </summary>
        private Dictionary<string, object> queryFieldsDictionary = new Dictionary<string, object>();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    [Mapper]
    public class BaseEO:EntityBase
    {
        public string id { get; set; }
    }

    [Mapper]
    public class Student:BaseEO
    {
        [Mapper("studentName")]
        public string name { get; set; }
        public int age { get; set; }
    }

    public class BaseDTO
    {
        public string id { get; set; }
    }
    public class StudentDTO:BaseDTO
    {
        public string studentName { get; set; }
        public int age { get; set; }
    }                                                                                                                                                                                                                                                                                                      

    class Program
    {

        static void Main(string[] args)
        {
            Student s = new Student { id = "123456", name = "wuqiansen", age = 18 };

            StudentDTO t = CoffeeMapper<Student, StudentDTO>.AutoMap(s, (t1, t2)=> { t1.studentName = t2.name+"default"; });
        }
        
    }
}
