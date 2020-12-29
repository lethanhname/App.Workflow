using System.Threading.Tasks;
using App.Core.Workflow.Contract.Services;
using App.CoreLib.EF.Data.Entity;
using App.CoreLib.EF.Events;
namespace App.Core.Workflow.Business.Events
{
    public class WorkflowHandler : IEventHandler<AfterSaveEvent>
    {
        private readonly IWorkflowService wfService;
        public WorkflowHandler(IWorkflowService wfService)
        {
            this.wfService = wfService;
        }

        public async Task RunAsync(AfterSaveEvent obj)
        {
            foreach (var item in obj.Items)
            {
                var triggerResults = await wfService.TriggerAsync(item.EntityData as IdentityEntity, item.EntityType, item.State);
                foreach (var result in triggerResults)
                {
                    if (result.HasErrors)
                    {
                        foreach (var error in result.Errors)
                        {
                            item.Messages.Add(new CoreLib.EF.Messages.EntityError
                            {
                                Code = "",
                                Description = error
                            });
                        }
                    }
                }
            }
        }
    }
}