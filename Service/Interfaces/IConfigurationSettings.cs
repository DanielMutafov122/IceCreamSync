namespace Service.Interfaces;

public interface IConfigurationSettings
{
    string GetOAuthCompanyId();
    string GetOAuthUserId();
    string GetOAuthSecret();
    string GetApiUri();
}
