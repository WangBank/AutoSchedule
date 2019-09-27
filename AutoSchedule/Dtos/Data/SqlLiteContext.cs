using Microsoft.EntityFrameworkCore;

namespace AutoSchedule.Dtos.Data
{
    public class SqlLiteContext : DbContext
    {
        public SqlLiteContext(DbContextOptions<SqlLiteContext> options) : base(options)
        {

        }
        public DbSet<Models.Organization> OrgSetting { get; set; }
        public DbSet<Models.DataSource> OpenSql { get; set; }

        public DbSet<Models.SystemKey> SystemKeys { get; set; }
        public DbSet<Models.TaskPlan> TaskPlan { get; set; }

        public DbSet<Models.TaskPlanDetail> TaskPlanRelation { get; set; }
    }
}
