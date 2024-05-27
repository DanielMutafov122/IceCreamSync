using Domain.Entities;
using Models.Responses;

namespace Service.Mappers;

public static class WorkflowMapper
{
    public static Workflow ToWorkflowEntity(this WorkflowsResponse workflow)
    {
        return new Workflow
        {
            Id = workflow.Id,
            IsActive = workflow.IsActive,
            MultiExecBehavior = workflow.MultiExecBehavior,
            Name = workflow.Name,
            IsRunning = true
        };
    }
}
