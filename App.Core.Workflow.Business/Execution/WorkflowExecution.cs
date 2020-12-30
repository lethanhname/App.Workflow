using System.Collections.Generic;
using System.Linq;
using App.Core.Workflow.Contract.Definition;
using App.Core.Workflow.Contract.Execution;
using App.Core.Workflow.Contract.Items;
using App.CoreLib.EF.Data.Entity;

namespace App.Core.Workflow.Business.Execution
{
    public class WorkflowExecution
    {
        private readonly IWorkflowDefinition _definition;

        public WorkflowExecution(IWorkflowDefinition definition)
        {
            _definition = definition;
        }

        public IEnumerable<TriggerResult> GetTriggers(
          IWorkflow workflow,
          IdentityEntity instance,
          Dictionary<string, WorkflowVariableBase> variables = null)
        {
            var context = CreateTransitionContext(workflow, instance, variables);

            return _definition.Transitions
              .Where(t => t.State == workflow.State)
              .Select(t => CreateTriggerResult(t.Trigger, context, t)).ToList();
        }

        public TriggerResult CanTrigger(TriggerParam param)
        {
            var context = CreateTransitionContext(param.Workflow, param.Instance, param.Variables);

            return CanMakeTransition(context, param.TriggerName, param.Workflow);
        }

        public TriggerResult Trigger(TriggerParam param)
        {
            var context = CreateTransitionContext(param.Workflow, param.Instance, param.Variables);

            var result = CanMakeTransition(context, param.TriggerName, param.Workflow);
            if (!result.CanTrigger) return result;

            var transition = GetTransition(param.TriggerName, param.Workflow);

            if (context.TransitionAborted)
                return CreateTriggerResult(param.TriggerName, context, transition);

            var state = _definition.States.Single(s => s == transition.TargetState);
            param.Workflow.State = state;

            foreach (var action in transition.Actions)
            {
                action.Invoke(context);
            }
            result.SetAutoTrigger(transition.AutoTrigger?.Invoke(context));
            return result;
        }

        private static TransitionContext CreateTransitionContext(
          IWorkflow workflow,
          IdentityEntity instance,
          Dictionary<string, WorkflowVariableBase> variables)
        {
            var context = new TransitionContext(workflow, instance);
            if (variables != null)
            {
                foreach (var variable in variables)
                {
                    context.SetVariable(variable.Key, variable.Value);
                }
            }

            return context;
        }

        private static TriggerResult CreateTriggerResult(
          string triggerName,
          TransitionContext context,
          Transition transition)
        {
            var canTrigger = transition != null && transition.CanMakeTransition(context);

            return new TriggerResult(triggerName, context, canTrigger);
        }

        private Transition GetTransition(string triggerName, IWorkflow workflow)
        {
            return _definition.Transitions
              .SingleOrDefault(t => t.Trigger == triggerName && t.State == workflow.State);
        }

        private TriggerResult CanMakeTransition(
          TransitionContext context,
          string triggerName,
          IWorkflow instance)
        {
            var transition = GetTransition(triggerName, instance);
            var triggerResult = CreateTriggerResult(triggerName, context, transition);

            if (transition != null) return triggerResult;

            context.AddError($"Transition for trigger '{triggerName}' not found!");

            return triggerResult;
        }
    }
}