using System;
using System.Collections.Generic;
using System.Text;

namespace BankDbHelper
{
    public class ParamSql
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000005 RID: 5 RVA: 0x00002422 File Offset: 0x00000622
        // (set) Token: 0x06000006 RID: 6 RVA: 0x0000242A File Offset: 0x0000062A
        public string Sql { get; set; }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000007 RID: 7 RVA: 0x00002433 File Offset: 0x00000633
        // (set) Token: 0x06000008 RID: 8 RVA: 0x0000243B File Offset: 0x0000063B
        public List<SqlHelperParameter> Params { get; set; }

        // Token: 0x06000009 RID: 9 RVA: 0x00002444 File Offset: 0x00000644
        public ParamSql()
        {
            this.Sql = string.Empty;
            this.Params = new List<SqlHelperParameter>();
        }

        // Token: 0x0600000A RID: 10 RVA: 0x00002466 File Offset: 0x00000666
        public ParamSql(string sql, List<SqlHelperParameter> lst)
        {
            this.Sql = sql;
            this.Params = lst;
        }
    }
}
