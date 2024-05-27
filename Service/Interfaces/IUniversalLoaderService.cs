using Models.Responses;

namespace Service.Interfaces;

public interface IUniversalLoaderService
{
    Task<List<WorkflowsResponse>?> GetWorkFlows();

    Task<int> RunWorkflow(int workflowId);
}
