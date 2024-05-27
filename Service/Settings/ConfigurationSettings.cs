using Microsoft.Extensions.Options;
using Models;
using Service.Interfaces;

namespace Service.Settings;

public class ConfigurationSettings : IConfigurationSettings
{
    private readonly ConfigurationOptions _options;

    public ConfigurationSettings(IOptions<ConfigurationOptions> options)
    {
        _options = options.Value;
    }

    public string GetOAuthCompanyId() => _options.CompanyId;
    public string GetOAuthSecret() => _options.Secret;
    public string GetOAuthUserId() => _options.UserId;
    public string GetApiUri() => _options.ApiUri;
}
