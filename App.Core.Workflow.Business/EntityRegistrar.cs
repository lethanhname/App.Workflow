

using Microsoft.EntityFrameworkCore;
using App.CoreLib.EF.Data;
using App.Core.Workflow.Contract;
using App.Core.Workflow.Contract.Items;
using App.Core.Workflow.Contract.Scheduler;

namespace App.Core.Workflow.Business
{
    public class EntityService
    {
        public static void RegisterEntities(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkflowItem>(builder =>
            {
                builder.ToTable("Workflow");

                // colums
                builder.HasKey(x => x.IdentityId);
                builder.Property(x => x.IdentityId).ValueGeneratedOnAdd();
                builder.Property(x => x.WorkflowName).IsRequired();
                builder.Property(x => x.State).IsRequired();
                builder.Property(x => x.EntityId).IsRequired();
                builder.Property(e => e.RowVersion).IsConcurrencyToken();
                builder.HasIndex(p => new { p.WorkflowName, p.EntityId }).IsUnique();
            });

            modelBuilder.Entity<WorkflowHistory>(builder =>
            {
                // table
                builder.ToTable("WorkflowHistory");

                // colums
                builder.HasKey(x => x.IdentityId);
                builder.Property(x => x.IdentityId).ValueGeneratedOnAdd();
                builder.Property(x => x.Created).IsRequired();
                builder.Property(x => x.FromState).IsRequired();
                builder.Property(x => x.ToState).IsRequired();
                builder.Property(e => e.RowVersion).IsConcurrencyToken();
            });

            modelBuilder.Entity<WorkflowVariable>(builder =>
            {
                // table
                builder.ToTable("WorkflowVariable");

                // colums
                builder.HasKey(x => x.IdentityId);
                builder.Property(x => x.IdentityId).ValueGeneratedOnAdd();
                builder.Property(x => x.Type).IsRequired();
                builder.Property(x => x.Content).IsRequired();
                builder.Property(e => e.RowVersion).IsConcurrencyToken();
            });


            modelBuilder.Entity<WorkItem>(builder =>
            {
                // table
                builder.ToTable("WorkItem");

                // colums
                builder.HasKey(x => x.IdentityId);
                builder.Property(x => x.IdentityId).ValueGeneratedOnAdd();
                builder.Property(x => x.TriggerName).IsRequired();
                builder.Property(x => x.EntityId).IsRequired();
                builder.Property(x => x.WorkflowType).IsRequired();
                builder.Property(x => x.DueDate).IsRequired();
                builder.Property(e => e.RowVersion).IsConcurrencyToken();
            });
        }
    }
}