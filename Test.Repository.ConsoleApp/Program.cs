using System;
using System.Linq;
using FJW.CommonLib.Utils;
using FJW.Repository;
using FJW.Repository.Expression2Sql;
using System.Collections.Generic;

namespace Test.Repository.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IExpressionRepository expressionRepository = new ExpressionRepository("test");
            IRepository repository = expressionRepository;
 
            var t = new List< long>();
            var id = int.Parse("182");
            
            var ids = new long[]{1, 2, 3, 4};

            var oneObj = repository.Single<MemberEntity>(it => it.IsDelete == 0 && it.Id == id );

            var manyObj = repository.Query<MemberEntity>(it => it.IsDelete == 0 && ids.Contains(it.Id));

            Console.WriteLine(oneObj.Id);
            /*
           repository.Query<MemberEntity>(it => it.IsDelete == 0, 1, 5, out rowCount);
           
           Console.ReadLine();

           var entity = new MemberEntity {NickName = "Abc" + count, Phone = "130" + count.ToString("D8")};
           //repository.Add(entity);
           //repository.Context.Commit();
           //Console.WriteLine("entity.Id:{0}", entity.Id);//Commit()后，自增列可以返回Id
           //-------------------------------------------------------------------------------------
           entity.NickName = entity.GetHashCode().ToString();
           //repository.Update(entity);

           repository.UpdateOnly<MemberEntity>(() => new { entity.NickName }, m => m.Id == 1);

           repository.Context.Commit(); //提交 
           //repository.Context.Rollback(); //不进行提交，清除之前的命令
           //int rowCount;
           var members = repository.Query<MemberEntity>(it => it.IsDelete == 0, 0, 2, out rowCount);
           Console.WriteLine("members:\r\n{0}", JsonHelper.JsonSerializer(members));
           */
            
            

            Console.ReadLine();
        }
    }
}
