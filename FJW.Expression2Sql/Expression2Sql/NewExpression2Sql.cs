﻿#region License
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
using System.Linq.Expressions;
using System.Reflection;

namespace FJW.Expression2Sql.Expression2Sql
{
    class NewExpression2Sql : BaseExpression2Sql<NewExpression>
    {
        protected override SqlBuilder Where(NewExpression expression, SqlBuilder sqlBuilder)
        {
            return base.Where(expression, sqlBuilder);
        }

        protected override SqlBuilder Insert(NewExpression expression, SqlBuilder sqlBuilder)
        {
            string columns = " (";
            string values = " values (";

            for (int i = 0; i < expression.Members.Count; i++)
            {
                MemberInfo m = expression.Members[i];
                var defiend = ColumnCache.GetDefined(m.Name, sqlBuilder.MainTable);
                if (defiend.IsIdentity)
                {
                    continue;
                }
                columns += defiend.Name + ",";

                ConstantExpression c = expression.Arguments[i] as ConstantExpression;
                string dbParamName = sqlBuilder.AddDbParameter(c.Value, false);
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

        protected override SqlBuilder Update(NewExpression expression, SqlBuilder sqlBuilder)
        {

            for (int i = 0; i < expression.Members.Count; i++)
            {
                MemberInfo m = expression.Members[i];
                var d = ColumnCache.GetDefined(m.Name, sqlBuilder.MainTable);
                if (d.IsKey)
                {
                    sqlBuilder.IsIdentity = true;
                    continue;
                }
                object val = null;
                if (expression.Arguments[i].NodeType == ExpressionType.Constant)
                {
                    ConstantExpression c = expression.Arguments[i] as ConstantExpression;
                    if (c == null)
                    {
                        throw new Exception("Update NewExpression 无法解析 " + expression.Arguments[i].NodeType);
                    }
                    val = c.Value;
                }
                else if (expression.Arguments[i].NodeType == ExpressionType.MemberAccess)
                {
                    var mExp = expression.Arguments[i] as MemberExpression;
                    if (mExp == null)
                    {
                        throw new Exception("Update NewExpression 无法解析 " + expression.Arguments[i].NodeType);
                    }
                    var n = mExp.Member.Name;
                    if (mExp.Member.MemberType == MemberTypes.Field)
                    {
                        val = MemberExpression2Sql.GetValue(mExp);
                    }
                    else if (mExp.Member.MemberType == MemberTypes.Property)
                    {
                        var property = mExp.Member as PropertyInfo;
                        val = property.GetValue( mExp.Member );
                    }
                    else
                    {
                        throw new Exception("Update NewExpression 无法解析 MemberType:" + mExp.Member.MemberType);
                    }
                }
                else
                {
                    throw new Exception("Update NewExpression 无法解析 " + expression.Arguments[i].NodeType);
                }

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

        protected override SqlBuilder Select(NewExpression expression, SqlBuilder sqlBuilder)
        {
            foreach (Expression item in expression.Arguments)
            {
                Expression2SqlProvider.Select(item, sqlBuilder);
            }

            foreach (MemberInfo item in expression.Members)
            {
                sqlBuilder.SelectFieldsAlias.Add(item.Name);
            }

            return sqlBuilder;
        }

        protected override SqlBuilder GroupBy(NewExpression expression, SqlBuilder sqlBuilder)
        {
            foreach (Expression item in expression.Arguments)
            {
                Expression2SqlProvider.GroupBy(item, sqlBuilder);
            }
            return sqlBuilder;
        }

        protected override SqlBuilder OrderBy(NewExpression expression, SqlBuilder sqlBuilder)
        {
            foreach (Expression item in expression.Arguments)
            {
                Expression2SqlProvider.OrderBy(item, sqlBuilder);
            }
            return sqlBuilder;
        }

        protected override SqlBuilder OrderByDescending(NewExpression expression, SqlBuilder sqlBuilder)
        {
            foreach (Expression item in expression.Arguments)
            {
                Expression2SqlProvider.OrderByDescending(item, sqlBuilder);
            }
            return sqlBuilder;
        }
    }
}
