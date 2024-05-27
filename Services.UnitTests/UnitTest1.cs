using System.Net.Http.Json;
using System.Net;
using Service.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Domain.Repositories.Interfaces;
using RichardSzalay.MockHttp;
using NSubstitute;
using Service;
using Models.Responses;
using AutoBogus;
using FluentAssertions;

namespace Services.UnitTests;

public class UniversalLoaderServiceTests
{
    private readonly MockHttpMessageHandler _http = new();
    private readonly IConfigurationSettings _configuration;
    private readonly IUniversalLoaderService _service;
    private readonly IMemoryCache _memoryCache;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly HttpClient _httpClient;

    public UniversalLoaderServiceTests()
    {
        _configuration = Substitute.For<IConfigurationSettings>();
        _memoryCache = Substitute.For<IMemoryCache>();
        _workflowRepository = Substitute.For<IWorkflowRepository>();
        _configuration.GetApiUri().Returns("https://testUri");
        _http.Expect(HttpMethod.Post, "https://testUri/authenticate").Respond(HttpStatusCode.OK, JsonContent.Create("token"));
        _httpClient = new HttpClient(_http);
        _service = new UniversalLoaderService(_httpClient, _configuration, _memoryCache, _workflowRepository);
    }

    [Fact]
    public async Task GetWorkflowsSuccessful()
    {
        var workFlowRespnse = new List<WorkflowsResponse>()
        {
            AutoFaker.Generate<WorkflowsResponse>()
        };

        _http.Expect(HttpMethod.Post, "https://testUri/workflow").Respond(HttpStatusCode.OK, JsonContent.Create(workFlowRespnse));
        var response = await _service.GetWorkFlows();

        response.Should().NotBeNull();
    }
}