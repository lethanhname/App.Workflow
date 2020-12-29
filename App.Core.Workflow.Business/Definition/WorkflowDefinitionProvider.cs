using System;
using System.Collections.Generic;
using System.Linq;
using App.Core.Workflow.Contract.Definition;

namespace App.Core.Workflow.Business.Definition
{
    public class WorkflowDefinitionProvider : IWorkflowDefinitionProvider
    {
        private readonly IEnumerable<IWorkflowDefinition> workflowDefinitions;

        public WorkflowDefinitionProvider(IEnumerable<IWorkflowDefinition> workflowDefinitions)
        {
            this.workflowDefinitions = workflowDefinitions;
        }

        public IWorkflowDefinition GetWorkflowDefinition(string workflowName)
        {
            return this.workflowDefinitions.First(t => t.WorkflowName == workflowName);
        }

        public IEnumerable<IWorkflowDefinition> GetWorkflowDefinitions()
        {
            return this.workflowDefinitions;
        }

        public IEnumerable<IWorkflowDefinition> GetWorkflowDefinitions(string entity)
        {
            return this.workflowDefinitions.Where(t => t.Entity == entity);
        }

        public void RegisterWorkflowDefinition(IWorkflowDefinition workflowDefinition)
        {
            throw new NotImplementedException();
        }
    }
}