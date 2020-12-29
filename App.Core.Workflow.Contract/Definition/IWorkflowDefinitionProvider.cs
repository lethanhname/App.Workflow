using System.Collections.Generic;

namespace App.Core.Workflow.Contract.Definition
{
    public interface IWorkflowDefinitionProvider
    {
        void RegisterWorkflowDefinition(IWorkflowDefinition workflowDefinition);

        IWorkflowDefinition GetWorkflowDefinition(string workflowName);

        IEnumerable<IWorkflowDefinition> GetWorkflowDefinitions(string Entity);

        IEnumerable<IWorkflowDefinition> GetWorkflowDefinitions();
    }
}