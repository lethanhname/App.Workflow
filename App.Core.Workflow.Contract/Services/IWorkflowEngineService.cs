using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Core.Workflow.Contract.Execution;
using App.Core.Workflow.Contract.Items;
using App.CoreLib.EF.Data.Entity;

namespace App.Core.Workflow.Contract.Services
{
    public interface IWorkflowEngineService
    {
        Task<IEnumerable<TriggerResult>> GetTriggersAsync(
          IWorkflow workflow,
          IdentityEntity instance,
          Dictionary<string, WorkflowVariableBase> variables = null
        );

        Task<TriggerResult> CanTriggerAsync(TriggerParam param);

        Task<TriggerResult> TriggerAsync(TriggerParam param);

        // IWorkflow Find(int id, Type type);
    }
}