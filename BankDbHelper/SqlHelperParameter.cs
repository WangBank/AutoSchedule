using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BankDbHelper
{
    public class SqlHelperParameter
    {
        // Token: 0x17000004 RID: 4
        // (get) Token: 0x0600002D RID: 45 RVA: 0x00003199 File Offset: 0x00001399
        // (set) Token: 0x0600002E RID: 46 RVA: 0x000031A1 File Offset: 0x000013A1
        public string Name { get; set; }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x0600002F RID: 47 RVA: 0x000031AA File Offset: 0x000013AA
        // (set) Token: 0x06000030 RID: 48 RVA: 0x000031B2 File Offset: 0x000013B2
        public ParamsType DataType { get; set; }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000031 RID: 49 RVA: 0x000031BB File Offset: 0x000013BB
        // (set) Token: 0x06000032 RID: 50 RVA: 0x000031C3 File Offset: 0x000013C3
        public ParameterDirection Direction { get; set; }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000033 RID: 51 RVA: 0x000031CC File Offset: 0x000013CC
        // (set) Token: 0x06000034 RID: 52 RVA: 0x000031D4 File Offset: 0x000013D4
        public object Value { get; set; }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000035 RID: 53 RVA: 0x000031DD File Offset: 0x000013DD
        // (set) Token: 0x06000036 RID: 54 RVA: 0x000031E5 File Offset: 0x000013E5
        public int Size { get; set; }

        // Token: 0x06000037 RID: 55 RVA: 0x000031EE File Offset: 0x000013EE
        public SqlHelperParameter()
        {
            this.Name = string.Empty;
            this.DataType = ParamsType.Varchar;
            this.Direction = ParameterDirection.Input;
            this.Value = string.Empty;
            this.Size = 4;
        }

        // Token: 0x06000038 RID: 56 RVA: 0x00003228 File Offset: 0x00001428
        public SqlHelperParameter(string name, ParamsType dataType, ParameterDirection direction, object value, int size)
        {
            this.Name = name;
            this.DataType = dataType;
            this.Direction = direction;
            this.Value = value;
            this.Size = size;
        }
    }
}
