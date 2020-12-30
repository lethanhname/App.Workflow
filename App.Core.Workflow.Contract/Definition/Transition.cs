using System;
using System.Collections.Generic;
using App.Core.Workflow.Contract.Execution;

namespace App.Core.Workflow.Contract.Definition
{
    public class Transition
    {
        public string State { get; set; }

        public string Trigger { get; set; }

        public string TargetState { get; set; }

        public Func<TransitionContext, bool> CanMakeTransition { get; set; }

        public List<Action<TransitionContext>> Actions { get; set; }

        public Func<TransitionContext, AutoTrigger> AutoTrigger { get; set; }

        public Transition()
        {
            CanMakeTransition = transitionContext => true;
        }
    }
}