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
using System.Linq.Expressions;

namespace FJW.Expression2Sql
{
    public abstract class BaseExpression2Sql<T> : IExpression2Sql where T : Expression
    {
       

        protected virtual SqlBuilder Insert(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.Insert method");
        }
        protected virtual SqlBuilder Update(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.Update method");
        }
        protected virtual SqlBuilder Select(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.Select method");
        }
        protected virtual SqlBuilder Join(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.Join method");
        }
        protected virtual SqlBuilder Where(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.Where method");
        }
        protected virtual SqlBuilder In(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.In method");
        }
        protected virtual SqlBuilder GroupBy(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.GroupBy method");
        }
        protected virtual SqlBuilder OrderBy(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.OrderBy method");
        }

        protected virtual SqlBuilder OrderByDescending(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.OrderByDescending method");
        }

        protected virtual SqlBuilder Max(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.Max method");
        }
        protected virtual SqlBuilder Min(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.Min method");
        }
        protected virtual SqlBuilder Avg(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.Avg method");
        }
        protected virtual SqlBuilder Count(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.Count method");
        }
        protected virtual SqlBuilder Sum(T expression, SqlBuilder sqlBuilder)
        {
            throw new NotImplementedException("Unimplemented " + typeof(T).Name + "2Sql.Sum method");
        }


        public SqlBuilder Insert(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return Insert((T)expression, sqlBuilder);
        }
        public SqlBuilder Update(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return Update((T)expression, sqlBuilder);
        }
        public SqlBuilder Select(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return Select((T)expression, sqlBuilder);
        }
        public SqlBuilder Join(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return Join((T)expression, sqlBuilder);
        }
        public SqlBuilder Where(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return Where((T)expression, sqlBuilder);
        }
        public SqlBuilder In(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return In((T)expression, sqlBuilder);
        }
        public SqlBuilder GroupBy(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return GroupBy((T)expression, sqlBuilder);
        }
        public SqlBuilder OrderBy(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return OrderBy((T)expression, sqlBuilder);
        }

        public SqlBuilder OrderByDescending(Expression expression, SqlBuilder sqlBuilder)
        {
            return OrderByDescending((T)expression, sqlBuilder);
        }

        public SqlBuilder Max(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return Max((T)expression, sqlBuilder);
        }
        public SqlBuilder Min(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return Min((T)expression, sqlBuilder);
        }
        public SqlBuilder Avg(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return Avg((T)expression, sqlBuilder);
        }
        public SqlBuilder Count(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return Count((T)expression, sqlBuilder);
        }
        public SqlBuilder Sum(Expression expression, SqlBuilder sqlBuilder)
        {
            //Debug();
            return Sum((T)expression, sqlBuilder);
        }

    }
}
