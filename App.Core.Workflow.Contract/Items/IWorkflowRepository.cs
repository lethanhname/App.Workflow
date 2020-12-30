using System;
using System.Threading.Tasks;
using App.Core.Workflow.Contract.Definition;
using App.CoreLib.EF.Data;

namespace App.Core.Workflow.Contract.Items
{

    public interface IWorkflowRepository : IRepository<WorkflowItem>
    {
        Task<WorkflowItem> FindAsync(int entityId, string workflowName);
    }
}