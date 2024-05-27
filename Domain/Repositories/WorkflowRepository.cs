using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories;

public class WorkflowRepository : IWorkflowRepository
{
    private readonly AppDbContext _context;
    public WorkflowRepository(AppDbContext context)
    {

        _context = context;
    }

    public async Task Add(Workflow entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Workflow entity)
    {
        var workflow = _context.Workflows.AsNoTracking().FirstOrDefault(o => o.Id == entity.Id);
        if (workflow != null)
        {
            workflow.Name = entity.Name;
            workflow.MultiExecBehavior = entity.MultiExecBehavior;
            workflow.IsActive = entity.IsActive;
            workflow.IsRunning = entity.IsRunning;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddRange(IEnumerable<Workflow> entities)
    {
        await _context.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    public List<Workflow> GetAll()
    {
        return _context.Workflows.AsNoTracking().ToList();
    }

    public Workflow GetById(int id)
    {
        return _context.Workflows.AsNoTracking().FirstOrDefault(o => o.Id == id);
    }

    public async Task Remove(Workflow entity)
    {
        _context.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
