
using System.Collections.Concurrent;

namespace FJW.Expression2Sql
{
    public class SelectAllCache
    {
        private static readonly ConcurrentDictionary<string, string> NameDictionary = new ConcurrentDictionary<string, string>();

        public static string GetSelectFormat(string table )
        {
           
            var n = table;
            if (NameDictionary.ContainsKey(n))
            {
                return NameDictionary[n];
            }            
            return "{0}*";
        }

        public static void SetSelectFormat(string table, string sqlFormat)
        {
            NameDictionary[table] = sqlFormat;
        }

    }
}
