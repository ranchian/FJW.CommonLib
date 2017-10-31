#region License
/**
 * Copyright (c) 2015, 何志祥 (strangecity@qq.com).
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * without warranties or conditions of any kind, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace FJW.Expression2Sql.Expression2Sql
{
    class MemberExpression2Sql : BaseExpression2Sql<MemberExpression>
    {
        internal static object GetValue(MemberExpression expr)
        {
            object value;
            var field = expr.Member as FieldInfo;
            if (field != null)
            {
                return field.GetValue(((ConstantExpression)expr.Expression).Value);
            }
            return ((PropertyInfo)expr.Member).GetValue(((ConstantExpression)expr.Expression).Value, null);

        }

        private SqlBuilder AggregateFunctionParser(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            string aggregateFunctionName = new StackTrace(true).GetFrame(1).GetMethod().Name.ToLower();
            string tableName = TableNameCache.GetName(expression.Expression.Type);
            //string tableName = expression.Member.DeclaringType.Name;
            string columnName = ColumnCache.GetDefined(expression.Member.Name, tableName).Name; //expression.Member.Name;

            sqlBuilder.SetTableAlias(tableName);
            string tableAlias = sqlBuilder.GetTableAlias(tableName);

            if (!string.IsNullOrWhiteSpace(tableAlias))
            {
                tableName += " " + tableAlias;
                columnName = tableAlias + "." + columnName;
            }
            sqlBuilder.AppendFormat("select {0}({1}) from {2}", aggregateFunctionName, columnName, tableName);
            return sqlBuilder;
        }

        protected override SqlBuilder Select(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            string tableName = TableNameCache.GetName(expression.Expression.Type);
            sqlBuilder.SetTableAlias(tableName);
            string tableAlias = sqlBuilder.GetTableAlias(tableName);
            if (!string.IsNullOrWhiteSpace(tableAlias))
            {
                tableAlias += ".";
            }
            var name = expression.Member.Name;
            var n = ColumnCache.GetDefined(expression.Member.Name, tableName).Name;
            if (!n.Equals(name, System.StringComparison.CurrentCultureIgnoreCase))
            {
                sqlBuilder.SelectFields.Add(string.Format("{0}{1}", tableAlias, n));
                sqlBuilder.SelectFieldsAlias.Add(name);
            }
            else
            {
                sqlBuilder.SelectFields.Add(tableAlias + n); //expression.Member.Name);
                sqlBuilder.SelectFieldsAlias.Add(string.Empty);
            }


            return sqlBuilder;
        }

        protected override SqlBuilder Join(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            string tableName;
            if (expression.Expression.NodeType == ExpressionType.Parameter)
            {
                var exp = expression.Expression as ParameterExpression;
                if (exp == null)
                {
                    throw new Exception("Join MemberExpression 解析错误");
                }
                tableName = TableNameCache.GetName(exp.Type);
            }
            else
            {
                tableName = TableNameCache.GetName(expression.Member.DeclaringType);
            }
            sqlBuilder.SetTableAlias(tableName);
            var tableAlias = sqlBuilder.GetTableAlias(tableName);
            if (!string.IsNullOrWhiteSpace(tableAlias))
            {
                tableAlias += ".";
            }
            sqlBuilder += " " + tableAlias + ColumnCache.GetDefined(expression.Member.Name, tableName).Name; //expression.Member.Name;

            return sqlBuilder;
        }

        protected override SqlBuilder Where(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            if (expression.Expression.NodeType == ExpressionType.Constant)
            {
                object value = GetValue(expression);
                sqlBuilder.AddDbParameter(value);
            }
            else if (expression.Expression.NodeType == ExpressionType.Parameter)
            {
                //string tableName = TableNameCache.GetName(expression.Member.DeclaringType);
                var tableName = TableNameCache.GetName(expression.Expression.Type);
                sqlBuilder.SetTableAlias(tableName);
                string tableAlias = sqlBuilder.GetTableAlias(tableName);
                if (!string.IsNullOrWhiteSpace(tableAlias))
                {
                    tableAlias += ".";
                }
                sqlBuilder += " " + tableAlias + ColumnCache.GetDefined(expression.Member.Name, tableName).Name; //expression.Member.Name;
            }
            else if (expression.Expression.NodeType == ExpressionType.MemberAccess)
            {
                var exp = expression.Expression as MemberExpression;
                if (exp == null)
                {
                    throw new Exception("Where  MemberExpression 解析错误");
                }
                if (exp.Member.MemberType == MemberTypes.Property)
                {
                    var property = exp.Member as PropertyInfo;
                    var val = property.GetValue(exp.Member);
                    sqlBuilder.AddDbParameter(val);
                }
                else if (exp.Member.MemberType == MemberTypes.Field)
                {

                    var field = exp.Member as FieldInfo;

                    var constExp = (ConstantExpression)exp.Expression;
                    var value = constExp.Value;
                    var val = field.GetValue(value);
                    var t = val.GetType();
                    if (t.IsClass)
                    {
                        var name = expression.Member.Name;
                        sqlBuilder.AddDbParameter(GetValue(val, t, name));
                    }
                    else
                    {
                        sqlBuilder.AddDbParameter(val);
                    }

                }
                else
                {
                    throw new NotSupportedException("Where  MemberExpression 解析错误 :" + exp.Member.MemberType);
                }

            }
            return sqlBuilder;
        }

        public object GetValue(object val, Type t, string memberName)
        {
            var m = t.GetMember(memberName);
            if (m[0].MemberType == MemberTypes.Field)
            {
                var f = m[0] as FieldInfo;
                var realVal = f.GetValue(val);
                return realVal;
            }
            if (m[0].MemberType == MemberTypes.Property)
            {
                var p = m[0] as PropertyInfo;
                var realVal = p.GetValue(val);
                return realVal;
            }
            throw new NotSupportedException();
        }

        protected override SqlBuilder In(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            var field = expression.Member as FieldInfo;
            if (field != null)
            {
                object val = field.GetValue(((ConstantExpression)expression.Expression).Value);

                if (val != null)
                {
                    string itemJoinStr = "";
                    IEnumerable array = val as IEnumerable;
                    foreach (var item in array)
                    {
                        if (field.FieldType.Name == "String[]")
                        {
                            itemJoinStr += string.Format(",'{0}'", item);
                        }
                        else
                        {
                            itemJoinStr += string.Format(",{0}", item);
                        }
                    }

                    if (itemJoinStr.Length > 0)
                    {
                        itemJoinStr = itemJoinStr.Remove(0, 1);
                        itemJoinStr = string.Format("({0})", itemJoinStr);
                        sqlBuilder += itemJoinStr;
                    }
                }
            }

            return sqlBuilder;
        }

        protected override SqlBuilder GroupBy(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            string tableName = TableNameCache.GetName(expression.Member.DeclaringType);
            sqlBuilder.SetTableAlias(tableName);
            sqlBuilder += sqlBuilder.GetTableAlias(tableName) + "." + ColumnCache.GetDefined(expression.Member.Name, tableName).Name; //expression.Member.Name;
            return sqlBuilder;
        }

        protected override SqlBuilder OrderBy(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            string tableName;
            if (expression.Expression.NodeType == ExpressionType.Parameter)
            {
                var mberExp = expression.Expression as ParameterExpression;
                tableName = TableNameCache.GetName(mberExp.Type);
            }
            else
            {
                tableName = TableNameCache.GetName(expression.Member.DeclaringType);
            }

            sqlBuilder.SetTableAlias(tableName);
            if (!sqlBuilder.IsAppendedOrderBy)
            {
                sqlBuilder.IsAppendedOrderBy = true;
                sqlBuilder.OrderByText = "order by ";
                sqlBuilder += "\norder by ";
            }
            else
            {
                sqlBuilder += ", ";
                sqlBuilder.OrderByText += ", ";
            }
            var t = sqlBuilder.GetTableAlias(tableName) + "." + ColumnCache.GetDefined(expression.Member.Name, tableName).Name; //expression.Member.Name;
            sqlBuilder.OrderByText += t;
            sqlBuilder += t;
            return sqlBuilder;
        }

        protected override SqlBuilder OrderByDescending(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            string tableName;
            if (expression.Expression.NodeType == ExpressionType.Parameter)
            {
                var mberExp = expression.Expression as ParameterExpression;
                tableName = TableNameCache.GetName(mberExp.Type);
            }
            else
            {
                tableName = TableNameCache.GetName(expression.Member.DeclaringType);
            }
            sqlBuilder.SetTableAlias(tableName);
            if (!sqlBuilder.IsAppendedOrderBy)
            {
                sqlBuilder.IsAppendedOrderBy = true;
                sqlBuilder.OrderByText = "order by ";
                sqlBuilder += "\norder by ";
            }
            else
            {
                sqlBuilder += ", ";
                sqlBuilder.OrderByText += ", ";
            }
            var t = sqlBuilder.GetTableAlias(tableName) + "." + ColumnCache.GetDefined(expression.Member.Name, tableName).Name /* expression.Member.Name */ + " desc";
            sqlBuilder.OrderByText += t;
            sqlBuilder += t;
            return sqlBuilder;
        }

        protected override SqlBuilder Update(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            var constant = expression.Expression as ConstantExpression;
            if (constant == null)
            {
                throw new Exception("Update MemberExpression 解析不正确");
            }
            var valueObj = GetValue(expression);


            var t = valueObj.GetType();

            if (!t.IsClass)
            {
                throw new Exception("Update MemberExpression 参数不是有效的 Class");
            }

            var properties = t.GetProperties();
            foreach (var p in properties)
            {
                var d = ColumnCache.GetDefined(p.Name, sqlBuilder.MainTable);
                if (d.IsKey)
                {
                    sqlBuilder.IsIdentity = true;
                    continue;
                }
                var val = p.GetValue(valueObj);

                sqlBuilder += d.Name + " =";
                sqlBuilder.AddDbParameter(val);
                sqlBuilder += ",";
            }
            if (sqlBuilder[sqlBuilder.Length - 1] == ',')
            {
                sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            }
            return sqlBuilder;
        }


        protected override SqlBuilder Insert(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            string columns = " (";
            string values = " values (";


            //var t = expression.Type;

            //var nodeType = expression.Expression.NodeType;
            var constant = expression.Expression as ConstantExpression;
            if (constant == null)
            {
                throw new Exception("Insert MemberExpression 解析不正确");
            }
            var valueObj = GetValue(expression);

            var t = valueObj.GetType();

            if (!t.IsClass)
            {
                throw new Exception("Insert MemberExpression 参数不是有效的 Class");
            }

            var properties = t.GetProperties();
            foreach (var p in properties)
            {
                var defiend = ColumnCache.GetDefined(p.Name, sqlBuilder.MainTable);
                if (defiend.IsIdentity)
                {
                    sqlBuilder.IsIdentity = true;
                    continue;
                }
                var val = p.GetValue(valueObj);
                if (val == null)
                {
                    continue;
                }
                columns += defiend.Name + ",";

                var dbParamName = sqlBuilder.AddDbParameter(val, false);
                values += dbParamName + ",";
            }
            if (columns[columns.Length - 1] == ',')
            {
                columns = columns.Remove(columns.Length - 1, 1);
            }
            columns += ")";

            if (values[values.Length - 1] == ',')
            {
                values = values.Remove(values.Length - 1, 1);
            }
            values += ")";

            sqlBuilder += columns + values;

            return sqlBuilder;
        }

        protected override SqlBuilder Max(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            return AggregateFunctionParser(expression, sqlBuilder);
        }

        protected override SqlBuilder Min(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            return AggregateFunctionParser(expression, sqlBuilder);
        }

        protected override SqlBuilder Avg(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            return AggregateFunctionParser(expression, sqlBuilder);
        }

        protected override SqlBuilder Count(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            return AggregateFunctionParser(expression, sqlBuilder);
        }

        protected override SqlBuilder Sum(MemberExpression expression, SqlBuilder sqlBuilder)
        {
            return AggregateFunctionParser(expression, sqlBuilder);
        }
    }
}