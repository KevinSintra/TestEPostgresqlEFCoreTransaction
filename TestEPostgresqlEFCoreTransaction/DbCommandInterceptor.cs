using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using System.Data;
using System.Text.RegularExpressions;

namespace TestEPostgresqlEFCoreTransaction
{
    //public class WithLockDbCommandInterceptor : DbCommandInterceptor
    //{
    //    private static readonly Regex TableAliasRegex =
    //        new Regex(@"(?<tableAlias>AS \[[a-zA-Z]\w*\](?! WITH \(NOLOCK\)))",
    //                RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

    //    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    //    {
    //        command.CommandText = ReplaceCommandText(command);
    //        return result;
    //    }

    //    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
    //        CancellationToken cancellationToken = new CancellationToken())
    //    {
    //        command.CommandText = ReplaceCommandText(command);
    //        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    //    }

    //    private static string ReplaceCommandText(IDbCommand command)
    //    {
    //        return TableAliasRegex.Replace(command.CommandText, "${tableAlias} WITH (NOLOCK)");
    //    }
    //}
}
