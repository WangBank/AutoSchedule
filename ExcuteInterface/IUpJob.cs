using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ExcuteInterface
{
	public interface IUpJob
	{
		// Token: 0x0600000D RID: 13
		Task<int> ExecJob(JobPara jobParams, List<Datas> dsData);
	}
	public class Datas
	{
		public DataTable DataMain { get; set; }
		public List<DataTable> DataDetail { get; set; }
	}

	public class JobPara
	{
		public string connString { get; set; }
		public string dbType { get; set; }

		public string jobCode { get; set; }

	}


}
