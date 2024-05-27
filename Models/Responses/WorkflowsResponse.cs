namespace Models.Responses;

public class WorkflowsResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreationDateTime { get; set; }
    public int CreationUserId { get; set; }
    public int OwnerUserId { get; set; }
    public string MultiExecBehavior { get; set; } = string.Empty;
    public int? ExecutionRetriesCount { get; set; }
    public int? ExecutionRetriesPeriod { get; set; }
    public int? ExecutionRetriesPeriodTimeUnit { get; set; }
    public int WorkflowGroupId { get; set; }
    public bool CanStoreSuccessExecutionData { get; set; }
    public int? SuccessExecutionDataRetentionPeriodDays { get; set; }
    public bool CanStoreWarningExecutionData { get; set; }
    public int? WarningExecutionDataRetentionPeriodDays { get; set; }
    public bool CanStoreFailureExecutionData { get; set; }
    public int? FailureExecutionDataRetentionPeriodDays { get; set; }
}
