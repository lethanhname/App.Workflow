using System;
using System.Threading.Tasks;
using App.CoreLib.EF.Data;

namespace App.Core.Workflow.Contract.Items
{

    public interface IWorkflowRepository : IRepository<WorkflowItem>
    {
        Task<WorkflowItem> FindAsync(int entityId, string workflowName);

        // void AddAutoTrigger(AutoTrigger autoTrigger, IWorkflowInstanceEntity entity);
    }
}