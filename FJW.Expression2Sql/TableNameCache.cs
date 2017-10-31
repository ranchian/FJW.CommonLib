using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;

namespace FJW.Expression2Sql
{
    public class TableNameCache
    {
        private static  readonly ConcurrentDictionary<string, string> NameDictionary  = new ConcurrentDictionary<string, string>();

        public static string GetName(Type t)
        {
            var n = t.Name;
            if (NameDictionary.ContainsKey(n))
            {
                return NameDictionary[n];
            }
            var attres = t.GetCustomAttributes(typeof (TableAttribute), false);
            if (attres.Length > 0)
            {
                var table = attres[0] as TableAttribute;
                if (table != null)
                {
                   n = table.Name;
                }
            }
            NameDictionary[n] = n;
            ColumnCache.AnalysisTable(t, n);
            return n;
        }
    }
}
