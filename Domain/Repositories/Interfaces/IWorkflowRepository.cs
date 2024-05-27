using Domain.Entities;

namespace Domain.Repositories.Interfaces;

public interface IWorkflowRepository
{
    Workflow GetById(int id);
    List<Workflow> GetAll();
    Task Add(Workflow entity);
    Task AddRange(IEnumerable<Workflow> entities);
    Task Remove(Workflow entity);

    Task Update(Workflow entity);
}
