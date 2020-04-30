using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Models
{
    public interface IScopeTest
    {
        Guid OperationId { get; }
        Guid Setguid(Guid guid);
    }

    public class Operation : IScopeTest
    {
        private Guid _guid;
        public Operation()
        {
            if (_guid ==null)
            {
                _guid = Guid.NewGuid();
            }  
        }

        public Guid Setguid(Guid guid)
        {
            _guid = guid;
            return _guid;
        }
        public Guid OperationId => _guid;

    }
}
