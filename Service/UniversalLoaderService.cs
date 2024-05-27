using Domain.Entities;
using Domain.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Models.Responses;
using Newtonsoft.Json;
using Service.Interfaces;
using Service.Mappers;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Service;

public class UniversalLoaderService : IUniversalLoaderService
{
    private readonly HttpClient httpClient;
    private readonly IConfigurationSettings configurations;
    private readonly IMemoryCache memoryCache;
    private readonly IWorkflowRepository repository;
    private const string JwtTokenCache = "jwtTokenCache";
    public UniversalLoaderService(HttpClient httpClient, IConfigurationSettings configurations, IMemoryCache memoryCache, IWorkflowRepository repository)
    {
        this.httpClient = httpClient;
        this.configurations = configurations;
        this.memoryCache = memoryCache;
        this.repository = repository;
    }

    public async Task<List<WorkflowsResponse>?> GetWorkFlows()
    {
        httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", await GetJwtToken());
        var test = configurations.GetApiUri() + "/workflows";

        var result = await httpClient.GetAsync(configurations.GetApiUri() + "/workflows");

        if (result.IsSuccessStatusCode)
        {
            var workflows = JsonConvert.DeserializeObject<List<WorkflowsResponse>>(await result.Content.ReadAsStringAsync());
            var dbworkflows = repository.GetAll();
            if (dbworkflows != null && workflows != null)
            {
                if (!dbworkflows.Any())
                {
                    await repository.AddRange(workflows.Select(o => o.ToWorkflowEntity()));
                }
                else
                {
                    await UpdateWorkflows(workflows, dbworkflows);
                    await AddWorkflows(workflows, dbworkflows);
                    await DeleteOldWorkflows(workflows, dbworkflows);
                }
            }
            return workflows;
        }
        return new List<WorkflowsResponse>();
    }

    public async Task<int> RunWorkflow(int workflowId)
    {
        httpClient.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", await GetJwtToken());

        var result = await httpClient.PostAsync(configurations.GetApiUri() + $"/workflows/{workflowId}/run", null);

        return (int)result.StatusCode;
    }

    private async Task<string> GetJwtToken()
    {
        var token = memoryCache.Get<string?>(JwtTokenCache);

        if (token == null || !CheckTokenIsValid(token))
        {
            var body = new
            {
                apiCompanyId = configurations.GetOAuthCompanyId(),
                apiUserId = configurations.GetOAuthUserId(),
                apiUserSecret = configurations.GetOAuthSecret(),
            };

            var result = await httpClient.PostAsJsonAsync(configurations.GetApiUri() + $"/authenticate", body);

            if (!result.IsSuccessStatusCode)
            {
                return null!;
            }
            token = await result.Content.ReadAsStringAsync();

            var expirationTime = DateTimeOffset.Now.AddHours(1);
            memoryCache.Set(JwtTokenCache, token, expirationTime);
        }

        return token;
    }

    private static long GetTokenExpirationTime(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
        var ticks = long.Parse(tokenExp);
        return ticks;
    }

    private static bool CheckTokenIsValid(string token)
    {
        var tokenTicks = GetTokenExpirationTime(token);
        var tokenDate = DateTimeOffset.FromUnixTimeSeconds(tokenTicks).UtcDateTime;

        var now = DateTime.Now.ToUniversalTime();

        var valid = tokenDate >= now;

        return valid;
    }

    private async Task DeleteOldWorkflows(List<WorkflowsResponse> workflows, List<Workflow> dbWorkflows)
    {
        foreach (var workflow in dbWorkflows)
        {
            if (workflows.FirstOrDefault(o => o.Id == workflow.Id) == null)
            {
                await repository.Remove(workflow);
            }
        }
    }

    private async Task AddWorkflows(List<WorkflowsResponse> workflows, List<Workflow> dbWorkflows)
    {
        foreach (var workflow in workflows)
        {
            if (dbWorkflows.FirstOrDefault(o => o.Id == workflow.Id) == null)
            {
                await repository.Add(workflow.ToWorkflowEntity());
            }
        }
    }

    private async Task UpdateWorkflows(List<WorkflowsResponse> workflows, List<Workflow> dbWorkflows)
    {
        foreach (var workflow in workflows)
        {
            if (dbWorkflows.FirstOrDefault(o => o.Id == workflow.Id) != null)
            {
                await repository.Update(workflow.ToWorkflowEntity());
            }
        }
    }
}
