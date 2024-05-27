namespace Domain.Entities;

public class Workflow
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsRunning { get; set; }
    public string MultiExecBehavior { get; set; } = string.Empty;
}
