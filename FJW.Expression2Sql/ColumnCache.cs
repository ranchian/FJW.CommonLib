using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

namespace FJW.Expression2Sql
{
    /// <summary>
    /// 列缓存
    /// </summary>
    public class ColumnCache
    {
        private static readonly ConcurrentDictionary<string, ColumnDefined> NameDictionary = new ConcurrentDictionary<string, ColumnDefined>();

        public static ColumnDefined GetDefined(string name , string table)
        {
            var n = string.Format("{0}.{1}", table, name);
            if (NameDictionary.ContainsKey(n))
            {
                return NameDictionary[n];
            }
            var defined = new ColumnDefined { Name = name };           
            return defined;
        }

        public static void AnalysisTable( Type t, string table)
        {
            var properties = t.GetProperties();
            var sqlString = new StringBuilder();
            foreach (var p in properties)
            {
                AnalysisColumn(p, table, sqlString);
            }
            if (sqlString.Length > 0)
            {
                sqlString.Remove(sqlString.Length - 1, 1);
            }
            SelectAllCache.SetSelectFormat(table, sqlString.ToString());
        }

        private static void AnalysisColumn(MemberInfo info , string table, StringBuilder builder)
        {
            var defined = new ColumnDefined { Name = info.Name };
            var n = string.Format("{0}.{1}", table, info.Name);
            var attres1 = info.GetCustomAttributes(typeof(ColumnAttribute), false);
            if (attres1.Length > 0)
            {
                var c = attres1[0] as ColumnAttribute;
                if (c != null && !string.IsNullOrEmpty(c.Name))
                {
                    defined.Name = c.Name;
                }
            }
            var attres2 = info.GetCustomAttributes(typeof(KeyAttribute), false);
            if (attres2.Length > 0)
            {
                var t = attres2[0] as KeyAttribute;
                if (t != null)
                {
                    defined.IsKey = true;
                }
            }

            var attres3 = info.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), false);
            if (attres3.Length > 0)
            {
                var d = attres3[0] as DatabaseGeneratedAttribute;
                if (d != null && d.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                {
                    defined.IsIdentity = true;
                }
            }
            NameDictionary[n] = defined;
            if (defined.Name == info.Name)
            {
                builder.AppendFormat(" {{0}}{0},", defined.Name);
            }
            else
            {
                builder.AppendFormat(" {{0}}{0} as {1},", defined.Name, info.Name);
            }
        }


    }



    /// <summary>
    /// 列的定义
    /// </summary>
    public class ColumnDefined
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// 是否自证
        /// </summary>
        public bool IsIdentity { get; set; }
    }
}
