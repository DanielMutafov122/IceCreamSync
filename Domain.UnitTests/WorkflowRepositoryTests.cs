using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Domain.UnitTests;

public class WorkflowRepositoryTests
{
    private readonly DbContextOptions<AppDbContext> _options;

    public WorkflowRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
    }

    [Fact]
    public async Task Add_ShouldAddWorkflow()
    {
        using (var context = new AppDbContext(_options))
        {
            var repository = new WorkflowRepository(context);
            var workflow = new Workflow { Id = 1, Name = "Test Workflow" };

            await repository.Add(workflow);

            var addedWorkflow = await context.Workflows.FindAsync(workflow.Id);
            Assert.NotNull(addedWorkflow);
            Assert.Equal("Test Workflow", addedWorkflow.Name);
            context.Workflows.Remove(workflow);
            await context.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task Update_ShouldUpdateWorkflow()
    {
        using (var context = new AppDbContext(_options))
        {
            var workflow = new Workflow { Id = 1, Name = "Initial Workflow" };
            context.Workflows.Add(workflow);
            await context.SaveChangesAsync();

            var repository = new WorkflowRepository(context);
            workflow.Name = "Updated Workflow";

            await repository.Update(workflow);

            var updatedWorkflow = await context.Workflows.FindAsync(workflow.Id);
            Assert.NotNull(updatedWorkflow);
            Assert.Equal("Updated Workflow", updatedWorkflow.Name);
            context.Workflows.Remove(workflow);
            await context.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task AddRange_ShouldAddWorkflows()
    {
        using (var context = new AppDbContext(_options))
        {
            var repository = new WorkflowRepository(context);
            var workflows = new List<Workflow>
            {
                new Workflow { Id = 1, Name = "Workflow 1" },
                new Workflow { Id = 2, Name = "Workflow 2" }
            };

            await repository.AddRange(workflows);

            var addedWorkflows = context.Workflows.ToList();
            Assert.Equal(2, addedWorkflows.Count);
            Assert.Contains(addedWorkflows, w => w.Name == "Workflow 1");
            Assert.Contains(addedWorkflows, w => w.Name == "Workflow 2");
            context.Workflows.RemoveRange(workflows);
            await context.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllWorkflows()
    {
        using (var context = new AppDbContext(_options))
        {
            var workflows = new List<Workflow>
            {
                new Workflow { Id = 1, Name = "Workflow 1" },
                new Workflow { Id = 2, Name = "Workflow 2" }
            };
            context.Workflows.AddRange(workflows);
            context.SaveChanges();

            var repository = new WorkflowRepository(context);

            var result = repository.GetAll();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, w => w.Name == "Workflow 1");
            Assert.Contains(result, w => w.Name == "Workflow 2");
            context.Workflows.RemoveRange(workflows);
            await context.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task GetById_ShouldReturnWorkflow()
    {
        using (var context = new AppDbContext(_options))
        {
            var workflow = new Workflow { Id = 1, Name = "Workflow 1" };
            context.Workflows.Add(workflow);
            context.SaveChanges();

            var repository = new WorkflowRepository(context);

            var result = repository.GetById(1);

            Assert.NotNull(result);
            Assert.Equal("Workflow 1", result.Name);
            context.Workflows.Remove(workflow);
            await context.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task Remove_ShouldRemoveWorkflow()
    {
        using (var context = new AppDbContext(_options))
        {
            var workflow = new Workflow { Id = 1, Name = "Workflow 1" };
            context.Workflows.Add(workflow);
            context.SaveChanges();

            var repository = new WorkflowRepository(context);

            await repository.Remove(workflow);

            var removedWorkflow = await context.Workflows.FindAsync(workflow.Id);
            Assert.Null(removedWorkflow);
        }
    }
}