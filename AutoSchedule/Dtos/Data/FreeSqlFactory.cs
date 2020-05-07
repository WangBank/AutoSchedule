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
        private IFreeSql _messageOracle { get; set; }
        public IFreeSql GetMessageOracle()
        {
            if (_messageOracle == null)
            {
                _messageOracle = new FreeSqlBuilder()
                  .UseConnectionString(DataType.Oracle, Configuration.GetConnectionString("MessageDb"))
                  .UseAutoSyncStructure(false)
                  .Build();
            }
            return _messageOracle;
        }

        public List<IFreeSql> freeSqls { get; set; }
        private IFreeSql _baseSqlLite { get; set; }
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

        public IFreeSql CreateNewDb(DataType dataType, string SqlLiteConn)
        {
            return new FreeSqlBuilder()
              .UseConnectionString(dataType, SqlLiteConn)
              .UseAutoSyncStructure(false)
              .Build();
        }

    }
}
