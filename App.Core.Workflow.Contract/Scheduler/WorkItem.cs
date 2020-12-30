using System;
using App.CoreLib;
using App.CoreLib.EF.Data.Entity;

namespace App.Core.Workflow.Contract.Scheduler
{
    public partial class WorkItem : IdentityEntity
    {
        public static int WORKITEM_RETRIES = 3;

        public string TriggerName { get; set; }

        public int EntityId { get; set; }

        public string WorkflowType { get; set; }

        public int Retries { get; set; }

        public string Error { get; set; }
        public int IdentityId { get; set; }
        public int RowVersion { get; set; }

        public DateTime? DueDate { get; set; }


        public static WorkItem Create(
          string triggerName,
          int entityId,
          string workflowType,
          DateTime? dueDate = null
        )
        {
            return new WorkItem
            {
                TriggerName = triggerName,
                EntityId = entityId,
                WorkflowType = workflowType,
                Retries = 0,
                DueDate = dueDate ?? Globals.Now()
            };
        }
    }

}