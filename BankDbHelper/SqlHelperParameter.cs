using System.Data;

namespace BankDbHelper
{
    public class SqlHelperParameter
    {
        public string Name { get; set; }

        public ParamsType DataType { get; set; }

        public ParameterDirection Direction { get; set; }

        public object Value { get; set; }

        public int Size { get; set; }

        public SqlHelperParameter()
        {
            this.Name = string.Empty;
            this.DataType = ParamsType.Varchar;
            this.Direction = ParameterDirection.Input;
            this.Value = string.Empty;
            this.Size = 4;
        }

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