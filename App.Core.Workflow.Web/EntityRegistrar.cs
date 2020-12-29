using Microsoft.EntityFrameworkCore;
using App.CoreLib.EF.Data;
using App.Core.Workflow.Business;

namespace App.Core.Workflow.Web
{
    public class EntityRegistrar : IEntityRegistrar
    {
        public void RegisterEntities(ModelBuilder modelBuilder) => EntityService.RegisterEntities(modelBuilder);
    }
}