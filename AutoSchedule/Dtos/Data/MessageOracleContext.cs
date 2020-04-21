using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace AutoSchedule.Dtos.Data
{
    public class MessageOracleContext 
    {
       public IDbConnection dbConnection { get; set; }
    }
}
