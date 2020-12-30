using System.Collections.Generic;
using System.Linq;
using App.Core.Workflow.Contract.Definition;
using App.Core.Workflow.Contract.Items;

namespace App.Core.Workflow.Contract.Execution
{
    public class TriggerResult
    {
        private readonly TransitionContext _triggerContext;

        public bool CanTrigger { get; private set; }

        public bool IsAborted { get; private set; }

        public IWorkflow Workflow { get; private set; }

        public string TriggerName { get; private set; }

        public AutoTrigger AutoTrigger { get; private set; }

        public bool HasErrors
        {
            get { return Errors != null && Errors.Count() > 0; }
        }
        public bool HasAutoTrigger
        {
            get
            {
                return this.AutoTrigger != null;
            }
        }
        public IEnumerable<string> Errors { get; }

        public string CurrentState
        {
            get { return _triggerContext.Workflow.State; }
        }

        public TriggerResult(string triggerName, TransitionContext context, bool canTrigger)
        {
            TriggerName = triggerName;
            _triggerContext = context;
            CanTrigger = canTrigger;
            IsAborted = _triggerContext.TransitionAborted;
            Errors = _triggerContext.Errors;
        }

        public T GetVariable<T>(string key) where T : WorkflowVariableBase
        {
            if (_triggerContext == null) return null;

            return _triggerContext.GetVariable<T>(key);
        }

        public void SetAutoTrigger(AutoTrigger autoTrigger)
        {
            this.AutoTrigger = autoTrigger;
        }
    }
}