using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using App.CoreLib.EF.Data.Entity;

namespace App.Core.Workflow.Contract.Definition
{
    public interface IWorkflowDefinition
    {
        string WorkflowName { get; }

        string Entity { get; }

        List<string> States { get; }

        List<string> Triggers { get; }

        List<Transition> Transitions { get; }
    }
    public abstract class WorkflowDefinitionBase : IWorkflowDefinition
    {

        public abstract string WorkflowName { get; }

        public abstract string Entity { get; }

        private List<string> _states;

        public virtual List<string> States
        {
            get
            {
                if (_states != null)
                {
                    return _states;
                }

                var states = Transitions
                  .Select(t => t.State);

                var targetStates = Transitions
                  .Select(t => t.TargetState);

                _states = states
                  .Union(targetStates)
                  .Distinct()
                  .ToList();

                return _states;
            }
        }

        private List<string> _triggers;
        public virtual List<string> Triggers
        {
            get
            {
                if (_triggers != null)
                {
                    return _triggers;
                }

                _triggers = Transitions
                  .Select(t => t.Trigger)
                  .ToList();

                return _triggers;
            }
        }
        public abstract List<Transition> Transitions { get; }
    }
}