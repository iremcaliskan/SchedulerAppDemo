using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace SchedulerAppDemo.Models
{
    public class SchedulerContext : DbContext
    {
        public SchedulerContext(DbContextOptions<SchedulerContext> options)
           : base(options) {
        }
        public DbSet<SchedulerEvent> Events { get; set; }
        public DbSet<SchedulerRecurringEvent> RecurringEvents { get; set; }

    }
}
