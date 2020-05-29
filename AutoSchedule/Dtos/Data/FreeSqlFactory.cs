using FreeSql;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.Data
{
    public class FreeSqlFactory
    {
        public IConfiguration Configuration { get; }
        public FreeSqlFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }
       
        public List<IFreeSql> freeSqls { get; set; }
        private IFreeSql _baseSqlLite { get; set; }
        private IFreeSql _baseLogSqlLite { get; set; }
        public IFreeSql GetBaseSqlLite()
        {
            string SqlLiteConn = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                SqlLiteConn = Configuration.GetConnectionString("SqlLiteLinux");
            }
            else
            {
                SqlLiteConn = Configuration.GetConnectionString("SqlLiteWin");
            }
            if (_baseSqlLite == null)
            {
                _baseSqlLite = new FreeSqlBuilder()
                  .UseConnectionString(DataType.Sqlite, SqlLiteConn)
                  .UseAutoSyncStructure(false)
                  .Build();
            }
            return _baseSqlLite;
        }

        public IFreeSql GetBaseLogSqlLite()
        {
            string SqlLiteConn = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                SqlLiteConn = Configuration.GetConnectionString("SqlLiteLogLinux");
            }
            else
            {
                SqlLiteConn = Configuration.GetConnectionString("SqlLiteLogWin");
            }
            if (_baseLogSqlLite == null)
            {
                _baseLogSqlLite = new FreeSqlBuilder()
                  .UseConnectionString(DataType.Sqlite, SqlLiteConn)
                  .UseAutoSyncStructure(false)
                  .Build();
            }
            return _baseLogSqlLite;
        }

        public IFreeSql CreateNewDb(DataType dataType, string SqlLiteConn)
        {
            return new FreeSqlBuilder()
              .UseConnectionString(dataType, SqlLiteConn)
              .UseAutoSyncStructure(false)
              .Build();
        }

    }
}
