using OOM.Attributes;
using OOM.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OOM
{
    public sealed class CoffeeMapper<TIn, TOut> where TIn:class where TOut:class
    {
        private static readonly Func<TIn, TOut> funcCache = FuncFactory();
        public static TOut AutoMap(TIn InData, Action<TOut, TIn> action = null)
        {
            TOut _out = funcCache(InData);

            if (null != action) action(_out, InData);

            return _out;
        }
        private static Func<TIn, TOut> FuncFactory()
        {
            #region get Info through Reflection

            var _outType = typeof(TOut);
            var _inType = typeof(TIn);
            var _outTypeProperties = _outType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var _outTypePropertyNames = _outTypeProperties.Select(p => p.Name);
            
            #endregion

            #region some Expression class that can be repeat used

            //Student in
            var _inDeclare = Expression.Parameter(_inType, "_in");
            //StudentDTO _out
            var _outDeclare = Expression.Parameter(_outType, "_out");
            //new StudentDTO()
            var new_outEntityExpression = Expression.New(_outType);
            //default(StudentDTO)
            var default_outEntityValue = Expression.Default(_outType);
            //_in == null
            var _inEqualnullExpression = Expression.Equal(_inDeclare, Expression.Constant(null));

            #endregion

            var set_inEntityNotNullBlockExpressions = new List<Expression>();

            #region _out = new StudentDTO();
            set_inEntityNotNullBlockExpressions.Add(Expression.Assign(_outDeclare, new_outEntityExpression));
            #endregion

            PropertyInfo[] needMapPropertys = ScanAllPropertyNeedMap();

            foreach (var propertyInfo in needMapPropertys)
            {
                string mapToName = MapperAttribute.GetMapToPropertyName(propertyInfo);

                //no contain, no map
                if (!_outTypePropertyNames.Contains(mapToName))
                    continue;

                //no type equal, no map and expection
                if (_outTypeProperties.First(p => p.Name == mapToName).PropertyType.FullName != propertyInfo.PropertyType.FullName)
                    continue;

                if (propertyInfo.CanWrite)
                {
                    //_out.Id
                    var _outPropertyExpression = Expression.Property(_outDeclare, _outTypeProperties.First(p => p.Name == mapToName));
                    //_in.Id
                    var _inPropertyExpression = Expression.Property(_inDeclare, propertyInfo);

                    //_out.Id = _in.Id;
                    set_inEntityNotNullBlockExpressions.Add(

                        Expression.Assign(_outPropertyExpression, _inPropertyExpression)
                    );
                }
            }

            var checkIf_inIsNull = Expression.IfThenElse(
                _inEqualnullExpression,
                Expression.Assign(_outDeclare, default_outEntityValue),
                Expression.Block(set_inEntityNotNullBlockExpressions)
            );

            var body = Expression.Block(

                new[] { _outDeclare },
                checkIf_inIsNull,
                _outDeclare   //return _out;
            );

            return Expression.Lambda<Func<TIn, TOut>>(body, _inDeclare).Compile();
        }

        /// <summary>
        /// Get All Property Info that need be mapped
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <param name="InData"></param>
        /// <returns></returns>
        private static PropertyInfo[] ScanAllPropertyNeedMap()
        {
            List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();

            //取得包括當前層級類在內的所有繼承的每一層祖先類型
            List<Type> inheritClassList = new List<Type>();
            TypeExtension.GetInheritClassTypes(typeof(TIn), ref inheritClassList);

            foreach (Type classType in inheritClassList)
            {
                var attrs = classType.GetCustomAttributes(typeof(MapperAttribute), false);
                if (null == attrs || attrs.Count() <= 0) continue;

                PropertyInfo[] currentClassPropertyInfos = classType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
                                                                    .Where(proInfo => proInfo.GetCustomAttributes(typeof(NoMapperAttribute)).Count() <= 0)
                                                                    .ToArray();
                propertyInfoList.AddRange(currentClassPropertyInfos);
            }

            return propertyInfoList.ToArray();
        }

        #region
        //#region 實驗類型
        //public class EntityBase
        //{
        //    /// <summary>
        //    /// storage QueryField's Result
        //    /// </summary>
        //    private Dictionary<string, object> queryFieldsDictionary = new Dictionary<string, object>();

        //}

        //[Mapper]
        //public class BaseEO : EntityBase
        //{
        //    public string id { get; set; }
        //}
        //[Mapper]
        //public class Student : BaseEO
        //{
        //    [Mapper("studentName")]
        //    public string name { get; set; }
        //    public int age { get; set; }
        //    [NoMapper]
        //    public string nomapField { get; set; }
        //}

        //public class BaseDTO
        //{
        //    public string id { get; set; }
        //}
        //public class StudentDTO : BaseDTO
        //{
        //    public string studentName { get; set; }
        //    public int age { get; set; }
        //}
        //#endregion

        //public StudentDTO Student_AutoMapTo_StudentDTO(Student _in)
        //{
        //    StudentDTO _out;

        //    if (_in == null)
        //        _out = default(StudentDTO);
        //    else
        //    {
        //        _out = new StudentDTO();

        //        _out.id = _in.id;
        //        _out.age = _in.age;
        //        _out.studentName = _in.name;
        //    }

        //    return _out;
        //}
        #endregion

    }
}
