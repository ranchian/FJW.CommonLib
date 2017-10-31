using System;
using System.Collections;
using System.Xml;
using System.Data;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace FJW.CommonLib.ExtensionMethod
{
    public static class OtherExtensionMethods
    {
        #region DataTable,DataSet
        /// <summary>
        /// DataTable转Model
        /// </summary>
        /// <typeparam name="T">要转换的实体类型</typeparam>
        /// <param name="table">datatable</param>
        /// <returns>实体对象</returns>
        public static T DataTalbeToEntity<T>(this DataTable table) where T : new()
        {
            var entity = new T();
            try
            {
                if (table.Rows.Count < 1)
                {
                    return entity;
                }
                DataRow row = table.Rows[0];
                foreach (var item in entity.GetType().GetProperties())
                {
                    if (!row.Table.Columns.Contains(item.Name)) continue;
                    if (DBNull.Value == row[item.Name]) continue;
                    item.SetValue(entity, item.PropertyType.IsGenericType ? Convert.ChangeType(row[item.Name], item.PropertyType.GetGenericArguments()[0]) : Convert.ChangeType(row[item.Name], item.PropertyType), null);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("转换失败，原因：" + ex.Message);
            }

            return entity;
        }

        /// <summary>    
        /// 转化一个DataTable    
        /// </summary>    
        /// <typeparam name="T">集合类型</typeparam>    
        /// <param name="list">集合对象</param>    
        /// <returns>datatable</returns>    
        public static DataTable ToDataTable<T>(this IEnumerable<T> list)
        {
            DataTable dt = new DataTable();
            try
            {
                List<PropertyInfo> pList = new List<PropertyInfo>();
                Type type = typeof(T);
                Array.ForEach<PropertyInfo>(type.GetProperties(), p => { pList.Add(p); dt.Columns.Add(p.Name, p.PropertyType); });
                foreach (var item in list)
                {
                    DataRow row = dt.NewRow();
                    pList.ForEach(p => row[p.Name] = p.GetValue(item, null));
                    dt.Rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("转换失败，原因：" + ex.Message);
            }
            return dt;
        }

        /// <summary>    
        /// DataTable 转换为List 集合    
        /// </summary>    
        /// <typeparam name="T">类型</typeparam>    
        /// <param name="dt">DataTable</param>    
        /// <returns>集合</returns>    
        public static List<T> DataTableToList<T>(this DataTable dt) where T : class, new()
        {
            List<T> oblist = new List<T>();
            try
            {
                List<PropertyInfo> prlist = new List<PropertyInfo>();
                Type t = typeof(T);
                Array.ForEach<PropertyInfo>(t.GetProperties(), p => { if (dt.Columns.IndexOf(p.Name) != -1) prlist.Add(p); });
                foreach (DataRow row in dt.Rows)
                {
                    T ob = new T();
                    prlist.ForEach(p => { if (row[p.Name] != DBNull.Value) p.SetValue(ob, row[p.Name], null); });
                    oblist.Add(ob);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("转换失败，原因：" + ex.Message);
            }
            return oblist;
        }

        /// <summary>
        /// 验证DataSet是否为空
        /// </summary>
        /// <param name="ds">DataSet数据</param>
        /// <returns></returns>
        public static bool CheckDataSet(this DataSet ds)
        {
            bool isNull = false; //验证DS中所有的table是否都有值 
            if (ds != null && ds.Tables.Count > 0)
            {
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    if (ds.Tables[i].Rows.Count > 0)
                    {
                        isNull = true;
                    }
                }
                return isNull;
            }

            return false;

        }

        /// <summary>
        /// 验证DataTable是否为空
        /// </summary>
        /// <param name="ds">DataSet数据</param>
        /// <returns></returns>
        public static bool CheckDataTable(this DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region IList如何转成List<T>
        /// <summary>
        /// IList如何转成List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> IListToList<T>(this IList list)
        {
            T[] array = new T[list.Count];
            list.CopyTo(array, 0);
            return new List<T>(array);
        }
        #endregion

        #region List
        /// <summary>
        /// 获取集合中指定起始位置和结束位置的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="startindex">起始位置</param>
        /// <param name="pEndindex">结束位置（不包含）</param>
        /// <returns></returns>
        public static List<T> GetSubs<T>(this List<T> pValue, int pStartindex)
        {
            return GetSubs<T>(pValue, pStartindex, pValue.Count);
        }

        /// <summary>
        /// 获取集合中指定起始位置和结束位置的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="startindex">起始位置</param>
        /// <param name="pEndindex">结束位置（不包含）</param>
        /// <returns></returns>
        public static List<T> GetSubs<T>(this List<T> pValue, int pStartindex, int length)
        {
            List<T> list = new List<T> { };
            for (int i = pStartindex; i < ((pStartindex + length) < pValue.Count ? (pStartindex + length) : pValue.Count); i++)
            {
                list.Add(pValue[i]);
            }
            return list;
        }
        #endregion

        #region Type
        /// <summary>
        /// 扩展方法：判断类型是否为公共语言运行时类型
        /// </summary>
        /// <param name="pCaller">调用者</param>
        /// <returns></returns>
        public static bool IsCRT(this Type pCaller)
        {
            if (pCaller == null)
                throw new NullReferenceException();
            if (pCaller == typeof(bool)
                || pCaller == typeof(byte)
                || pCaller == typeof(char)
                || pCaller == typeof(DateTime)
                || pCaller == typeof(decimal)
                || pCaller == typeof(double)
                || pCaller == typeof(Int16)
                || pCaller == typeof(Int32)
                || pCaller == typeof(Int64)
                || pCaller == typeof(sbyte)
                || pCaller == typeof(Single)
                || pCaller == typeof(string)
                || pCaller == typeof(UInt16)
                || pCaller == typeof(UInt32)
                || pCaller == typeof(UInt64)
            )
                return true;
            else
                return false;
        }

        /// <summary>
        /// 扩展方法：判断类型是否为可空值类型
        /// </summary>
        /// <param name="pCaller">调用者</param>
        /// <returns></returns>
        public static bool IsNullableValueType(this Type pCaller)
        {
            if (pCaller == null)
                throw new NullReferenceException();
            if (pCaller.IsGenericType && pCaller.GetGenericTypeDefinition() == typeof(Nullable<>))
                return true;
            else
                return false;
        }

        /// <summary>
        /// 扩展方法：判断类型是否为基础类型为公共语言运行时类型的可空值类型
        /// </summary>
        /// <param name="pCaller">调用者</param>
        /// <returns></returns>
        public static bool IsNullableCRTValueType(this Type pCaller)
        {
            if (pCaller == null)
                throw new NullReferenceException();
            if (pCaller == typeof(bool?)
                || pCaller == typeof(byte?)
                || pCaller == typeof(char?)
                || pCaller == typeof(DateTime?)
                || pCaller == typeof(decimal?)
                || pCaller == typeof(double?)
                || pCaller == typeof(Int16?)
                || pCaller == typeof(Int32?)
                || pCaller == typeof(Int64?)
                || pCaller == typeof(sbyte?)
                || pCaller == typeof(Single?)
                || pCaller == typeof(UInt16?)
                || pCaller == typeof(UInt32?)
                || pCaller == typeof(UInt64?)
            )
                return true;
            else
                return false;
        }

        /// <summary>
        /// 扩展方法：获取可空值类型的基本类型
        /// </summary>
        /// <param name="pCaller"></param>
        /// <returns></returns>
        public static Type GetNullableUnderlyingType(this Type pCaller)
        {
            if (pCaller.IsNullableValueType())
                return pCaller.GetGenericArguments()[0];
            else
                return null;
        }
        #endregion

        #region Array
        /// <summary>
        /// 扩展方法：将数组中的字符串通过分隔符串联起来
        /// </summary>
        /// <param name="pCaller">调用者</param>
        /// <param name="pSpliter">分隔符</param>
        /// <param name="pWrapper">包装符,如果不为null,则数组中每个元素都用包装符在前后包装下.</param>
        /// <returns></returns>
        public static string ToJoinString<T>(this T[] pCaller, string pSpliter, string pWrapper = null)
        {
            if (pCaller == null)
                throw new NullReferenceException();
            var result = new StringBuilder();
            var hasWrapper = (!string.IsNullOrEmpty(pWrapper));
            if (pCaller.Length > 0)
            {
                for (var i = 0; i < pCaller.Length; i++)
                {
                    if (i != 0)
                        result.Append(pSpliter);
                    if (hasWrapper)
                        result.AppendFormat("{1}{0}{1}", pCaller[i], pWrapper);
                    else
                        result.Append(pCaller[i]);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 将字符串数组转换成In后面的条件
        /// </summary>
        /// <param name="pCaller">字符串数组</param>
        /// <returns>不带括号的In查询条件</returns>
        public static string ToSqlInString(this string[] pCaller)
        {
            string conditionIds = string.Empty;
            foreach (var conditionIdItem in pCaller)
            {
                conditionIds += "'" + conditionIdItem.Replace("'", "''") + "',";
            }
            if (conditionIds.Length > 0)
                conditionIds = conditionIds.Substring(0, conditionIds.Length - 1);
            return conditionIds;
        }
        #endregion

        #region XML
        /// <summary>
        /// 判断XML是否包含指定key的节点
        /// </summary>
        /// <param name="list"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsExists(this XmlNodeList list, string key)
        {
            bool i = false;
            foreach (XmlNode item in list)
            {
                if (item.Name == key)
                    return true;
            }
            return i;
        }

        /// <summary>
        /// 获取XML中指定key的节点
        /// </summary>
        /// <param name="list"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static XmlNode GetNode(this XmlNodeList list, string key)
        {
            foreach (XmlNode item in list)
            {
                if (item.Name == key)
                    return item;
            }
            return null;
        }
        #endregion
    }
}
