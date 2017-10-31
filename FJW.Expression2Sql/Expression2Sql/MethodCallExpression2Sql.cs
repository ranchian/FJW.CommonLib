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
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FJW.Expression2Sql.Expression2Sql
{
    class MethodCallExpression2Sql : BaseExpression2Sql<MethodCallExpression>
    {
        static Dictionary<string, Action<MethodCallExpression, SqlBuilder>> _Methods = new Dictionary<string, Action<MethodCallExpression, SqlBuilder>>
        {
            {"Like",Like},
            {"LikeLeft",LikeLeft},
            {"LikeRight",LikeRight},
            {"In",InnerIn}
        };

        private static void InnerIn(MethodCallExpression expression, SqlBuilder sqlBuilder)
        {
            Expression2SqlProvider.Where(expression.Arguments[0], sqlBuilder);
            sqlBuilder += " in";
            Expression2SqlProvider.In(expression.Arguments[1], sqlBuilder);
        }

        private static void Like(MethodCallExpression expression, SqlBuilder sqlBuilder)
        {
            if (expression.Object != null)
            {
                Expression2SqlProvider.Where(expression.Object, sqlBuilder);
            }
            Expression2SqlProvider.Where(expression.Arguments[0], sqlBuilder);
            sqlBuilder += " like '%' +";
            Expression2SqlProvider.Where(expression.Arguments[1], sqlBuilder);
            sqlBuilder += " + '%'";
        }

        private static void LikeLeft(MethodCallExpression expression, SqlBuilder sqlBuilder)
        {
            if (expression.Object != null)
            {
                Expression2SqlProvider.Where(expression.Object, sqlBuilder);
            }
            Expression2SqlProvider.Where(expression.Arguments[0], sqlBuilder);
            sqlBuilder += " like '%' +";
            Expression2SqlProvider.Where(expression.Arguments[1], sqlBuilder);
        }

        private static void LikeRight(MethodCallExpression expression, SqlBuilder sqlBuilder)
        {
            if (expression.Object != null)
            {
                Expression2SqlProvider.Where(expression.Object, sqlBuilder);
            }
            Expression2SqlProvider.Where(expression.Arguments[0], sqlBuilder);
            sqlBuilder += " like ";
            Expression2SqlProvider.Where(expression.Arguments[1], sqlBuilder);
            sqlBuilder += " + '%'";
        }

        protected override SqlBuilder Where(MethodCallExpression expression, SqlBuilder sqlBuilder)
        {
            var key = expression.Method;
            /*
            if (key.IsGenericMethod)
            {
                key = key.GetGenericMethodDefinition();
            }
            
            Action<MethodCallExpression, SqlBuilder> action;
            if (_Methods.TryGetValue(key.Name, out action))
            {
                action(expression, sqlBuilder);
                return sqlBuilder;
            }
            if (expression.Type != typeof(bool))
            {
                throw new NotImplementedException("Unimplemented method:" + expression.Method);
            }
            */

            var args = expression.Arguments;

            //var objExpressionNodeType = obj.NodeType;
            if (expression.Object != null && expression.Object.NodeType == ExpressionType.MemberAccess && args.Count== 1)
            {
                var memberExpression = expression.Object as MemberExpression;
                if (memberExpression == null)
                {
                    throw new Exception("Where MethodCallExpression 参数不正确");
                }
                var member = memberExpression.Member;

                var tableName = TableNameCache.GetName(member.DeclaringType);
                var columnName = ColumnCache.GetDefined(member.Name, tableName);
                var alias = sqlBuilder.GetTableAlias(tableName);
                sqlBuilder += string.Format(" {0}.{1} ", alias, columnName.Name);
                var arg = args[0] as ConstantExpression;
                if (arg == null)
                {
                    throw new ArgumentException("Where MethodCallExpression 参数不正确");
                }
                var v = arg.Value;

                switch (key.Name)
                {
                    case "StartsWith":
                        sqlBuilder += string.Format(" like {0} +'%'", sqlBuilder.AddDbParameter(v, false));
                        break;

                    case "EndsWith":
                        sqlBuilder += string.Format(" like '%' + {0}", sqlBuilder.AddDbParameter(v, false));
                        break;

                    case "Contains":
                        sqlBuilder += string.Format(" like '%'+ {0} +'%'", sqlBuilder.AddDbParameter(v, false));
                        break;
                }
                return sqlBuilder;
            }
            if (key.IsStatic && args.Count == 2)
            {

                switch (key.Name)
                {

                    case "Contains":

                        var arg0 = args[0];
                        var arg1 = args[1];
                        var arry = arg0.Type.IsArray ? arg0 : arg1;
                        var m = arg0.Type.IsArray ? arg1 : arg0;
                        var memberExpression = m as MemberExpression;
                        if (memberExpression == null)
                        {
                            throw new Exception("Where MethodCallExpression 参数不正确");
                        }
                        var member = memberExpression.Member;
                        var tableName = TableNameCache.GetName(memberExpression.Expression.Type);
                        var columnName = ColumnCache.GetDefined(member.Name, tableName);
                        var alias = sqlBuilder.GetTableAlias(tableName);
                        if (!string.IsNullOrEmpty(alias))
                        {
                            alias += ".";
                        }
                        sqlBuilder += string.Format(" {0}{1} in ( ", alias, columnName.Name);

                        if (arry.NodeType == ExpressionType.MemberAccess)
                        {
                            var mExp = arry as MemberExpression;
                            if (mExp == null)
                            {
                                throw new Exception("Where MethodCallExpression 参数不正确");
                            }

                            object val = null;
                            IEnumerable values = null;
                            if (mExp.Expression.NodeType == ExpressionType.Constant)
                            {
                                var constantExpression = mExp.Expression as ConstantExpression;
                                AssertNull(constantExpression);
                                val = constantExpression.Value;
                            }

                            if (mExp.Member.MemberType == MemberTypes.Field)
                            {
                                var f = mExp.Member as FieldInfo;
                                values = f.GetValue(val) as IEnumerable;
                            }
                            /*
                            if (mExp.Member.MemberType == MemberTypes.Property)
                            {
                                var mName = mExp.Member.Name;
                            }
                            */

                            foreach (var v in values)
                            {
                                sqlBuilder.AppendFormat("{0},", sqlBuilder.AddDbParameter(v, false));
                            }
                            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
                            sqlBuilder += ")";
                            return sqlBuilder;
                        }

                        break;
                }

                return sqlBuilder;
            }
            throw new NotImplementedException("Unimplemented method:" + expression.Method);
        }

        private void AssertNull(Expression expression)
        {
            if (expression == null)
            {
                throw new Exception("Where MethodCallExpression 参数不正确");
            }
        }
    }
}