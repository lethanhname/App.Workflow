namespace App.Core.Workflow.Contract.Items
{
    public interface IWorkflow
    {
        string WorkflowName { get; }

        string State { get; set; }

        int EntityId { get; set; }

        string Assignee { get; set; }
    }
}