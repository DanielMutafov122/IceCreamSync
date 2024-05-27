namespace Models;

public class ConfigurationOptions
{
    public const string Configuration = "Configuration";
    public string CompanyId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public string ApiUri { get; set; } = string.Empty;
}
