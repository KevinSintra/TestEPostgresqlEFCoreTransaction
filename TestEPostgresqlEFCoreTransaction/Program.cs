// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data;
using System.Text;
using TestEPostgresqlEFCoreTransaction;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

Console.WriteLine("Hello, World!");
//SelectUsingTransaction();
//TestReopsitorySelect();
//TestReopsitoryNoTrans();
TestReopsitoryHaveTrans();
Console.ReadLine();



// 測試 Transaction 情境為: LB 下是否能起到互相等待的效果
// 如何測? 開啟兩個 vs 2022 並且將其中一個中斷點放在 tran.Commit() 上，接者另外一個 IDE 執行從 console 會看到指跑到 Start 
void SelectUsingTransaction()
{
    using (var context = new TestDBContext())
    {
        var data = context.pg_stat_statements_record.AsNoTracking().FirstOrDefault(x => x.serialid == 1 && x.calls == 0);

        //using (var tran = context.Database.BeginTransaction()) // 兩個相同的更新可成功
        //using (var tran = context.Database.BeginTransaction(IsolationLevel.Snapshot)) // 兩個相同的更新不能成功 (不管 Query 是否在 Trans 下
        //using (var tran = context.Database.BeginTransaction(IsolationLevel.Serializable)) // 兩個相同的更新不能成功 (不管 Query 是否在 Trans 下
        //using (var tran = context.Database.BeginTransaction(IsolationLevel.RepeatableRead)) // 兩個相同的更新不能成功 (不管 Query 是否在 Trans 下
        //using (var tran = context.Database.BeginTransaction(IsolationLevel.ReadCommitted)) // 兩個相同的更新可成功 (不管 Query 是否在 Trans 下
        using (var tran = getIDbContextTransaction(context))
        {
            Console.WriteLine("Start");

            // transcation 查詢不管如何都不會去管
            //var data = context.pg_stat_statements_record.AsNoTracking().FirstOrDefault(x => x.serialid == 1 && x.calls == 0);
            var sb = new StringBuilder();
            sb.AppendLine($"serialid: {(data == null ? "not found" : data.serialid)}");
            sb.AppendLine($"calls: {(data == null ? "not found" : data.serialid)}");
            sb.AppendLine($"query: {data?.query ?? "not found"}");
            Console.WriteLine(sb.ToString());

            int updateReco = 0;

            EntityEntry<pg_stat_statements_record> entry;

            if (data != null)
            {
                data.calls = 1; // 可以查詢(值無變化) || 無法查詢(值必須有變化)

                entry = context.pg_stat_statements_record.Update(data);
                updateReco = context.SaveChanges();
            }

            tran.Commit();

            var a = 0;
        }
    }
}

// 取得 IDbContextTransaction，未來可運用在 Repository。可在制定新 Interface 供外部使用，自己去決定是否需要交易
Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction getIDbContextTransaction(TestDBContext testDBContext)
{
    var result = testDBContext.Database.BeginTransaction(IsolationLevel.RepeatableRead);

    return result;
}

void TestReopsitorySelect()
{
    using (var factory = new UnitOfWork(new TestDBContext()))
    {
        var data = factory.AppResources.Get(x => x.serialid > 80 && x.rows > 85).Result;

        var count = data.Count();

        Console.WriteLine($"select count: {count}");
    }
}

// 
void TestReopsitoryNoTrans()
{
    var date = DateTime.Now;
    var data = new pg_stat_statements_record { queryid = 111, query_date = date, calls = 5, total_exec_time = 5, min_exec_time = 5, max_exec_time = 5, rows = 5, query = "new data" };

    var data2 = new pg_stat_statements_record { queryid = 111, query_date = date, calls = 5, total_exec_time = 5, min_exec_time = 5, max_exec_time = 5, rows = 5, query = "new data" };

    var datas = new List<pg_stat_statements_record> { 
        new pg_stat_statements_record { queryid = 111, query_date = date, calls = 5, total_exec_time = 5, min_exec_time = 5, max_exec_time = 5, rows = 5, query = "new datas1" },
        new pg_stat_statements_record { queryid = 111, query_date = date, calls = 5, total_exec_time = 5, min_exec_time = 5, max_exec_time = 5, rows = 5, query = "new datas2" }
    };

    using (var factory = new UnitOfWork(new TestDBContext()))
    {
        var result = factory.AppResources.Add(data).Result;

        var result2 = factory.AppResources.Add(data2).Result;

        data.queryid = 112; data.query = "update";
        var result3 = factory.AppResources.Update(data).Result;

        var result4 = factory.AppResources.Delete(data2).Result;

        var result5 = factory.AppResources.Add(datas).Result;

        Console.WriteLine($"{result} {result2} {result3} {result4} {result5}");
    }
}

void TestReopsitoryHaveTrans()
{
    var date = DateTime.Now;
    var data = new pg_stat_statements_record { queryid = 111, query_date = date, calls = 5, total_exec_time = 5, min_exec_time = 5, max_exec_time = 5, rows = 5, query = "trans new data" };

    var data2 = new pg_stat_statements_record { queryid = 111, query_date = date, calls = 5, total_exec_time = 5, min_exec_time = 5, max_exec_time = 5, rows = 5, query = "trans new data2" };

    var datas = new List<pg_stat_statements_record> {
        new pg_stat_statements_record { queryid = 111, query_date = date, calls = 5, total_exec_time = 5, min_exec_time = 5, max_exec_time = 5, rows = 5, query = "trans new datas3" },
        new pg_stat_statements_record { queryid = 111, query_date = date, calls = 5, total_exec_time = 5, min_exec_time = 5, max_exec_time = 5, rows = 5, query = "trans new datas4" }
    };

    using (IUnitOfWork factory = new UnitOfWork(new TestDBContext()))
    {
        using (var trans = factory.GetRepoTransaction())
        {
            try
            {
                var result = factory.AppResources.Add(data).Result;

                var result2 = factory.AppResources.Add(data2).Result;

                data.queryid = 112; data.query = "trans update";
                var result3 = factory.AppResources.Update(data).Result;

                var result4 = factory.AppResources.Delete(data2).Result;

                var result5 = factory.AppResources.Add(datas).Result;

                Console.WriteLine($"before: {result} {result2} {result3} {result4} {result5}");

                var b = 0;

                var a = 1 / b;

                trans.CommitAsync().Wait();

                Console.WriteLine($"after: {result} {result2} {result3} {result4} {result5}");
            }
            catch (Exception)
            {
                trans.RollbackAsync().Wait();
                throw;
            }

        }

    }
}

//List<pg_stat_statements_record> pg_Stat_Statements_Records()
//{

//}