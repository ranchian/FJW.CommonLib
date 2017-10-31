using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FJW.Repository
{
    /// <summary>
    /// 数据单元基类
    /// </summary>
    public abstract class RepositoryContext : DisposableObject, IRepositoryContext
    {
        #region Private Fields
        private readonly Guid _id = Guid.NewGuid();

    
        private volatile bool _committed = true;
        #endregion

 

        #region Protected Methods

        /// <summary>
        /// 清除
        /// </summary>
        /// <remarks>提交后调用</remarks>
        protected virtual void ClearRegistrations()
        {
            
        }
 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearRegistrations();
            }
        }
        #endregion
         
        #region IRepositoryContext Members
    
        public Guid Id
        {
            get { return _id; }
    
        }
        
        public IClient Client { get; protected set; }

        public abstract void RegisterNew<T>(T obj) where T : BaseEntity;
 

        public abstract void RegisterModified<T>(T obj) where T : BaseEntity;

        public abstract void RegisterModified<T>(Expression<Func<T, bool>> predicate, Expression<Func<object>> onlyFields) where T : BaseEntity;

        public abstract void RegisterDeleted<T>(Expression<Func<T, bool>> predicate, T obj ) where T : BaseEntity , new();

        #endregion

        #region IUnitOfWork Members

        public virtual bool DistributedTransactionSupported
        {
            get { return false; }
        }
 
        public virtual bool Committed
        {
            get { return _committed; }
            protected set { _committed = value; }
        }
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public abstract void Commit(bool isTranscation = false);

        public Task CommitAsync()
        {
            return CommitAsync(CancellationToken.None);
        }

        public abstract Task CommitAsync(CancellationToken cancellationToken);

        public abstract void Rollback();



        #endregion
    }
}
