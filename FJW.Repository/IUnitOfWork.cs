using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace FJW.Repository
{
    /// <summary>
    /// 工作单元
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 是否支持分布式事务
        /// </summary>

        bool DistributedTransactionSupported { get; }
     

        /// <summary>
        /// 是否已经提交了
        /// </summary>
        bool Committed { get; }

        /// <summary>
        /// 提交
        /// </summary>
        void Commit(bool isTranscation = false);
        

        /// <summary>
        /// 异步提交
        /// </summary>
        /// <returns></returns>
        Task CommitAsync();
        
        /// <summary>
        /// 异步提交
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

        Task CommitAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 回滚
        /// </summary>
        void Rollback();
    }
}
