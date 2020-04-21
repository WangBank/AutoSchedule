using System;
using System.Collections.Generic;
using System.Data;
namespace ExcuteInterface
{
	public interface IUpJob
	{
		// Token: 0x0600000D RID: 13
		string ExecJob(JobPara jobParams, List<Datas> dsData,out string result);
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
