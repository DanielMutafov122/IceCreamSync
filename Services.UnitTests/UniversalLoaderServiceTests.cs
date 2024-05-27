using AutoBogus;
using Domain.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Models.Responses;
using NSubstitute;
using RichardSzalay.MockHttp;
using Service.Interfaces;
using Service;
using System.Net.Http.Json;
using System.Net;
using FluentAssertions;
using Domain.Entities;
using NSubstitute.ReturnsExtensions;

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
        _workflowRepository.GetAll().Returns(AutoFaker.Generate<List<Workflow>>());
        _http.Expect(HttpMethod.Get, "https://testuri/workflows").Respond(HttpStatusCode.OK, JsonContent.Create(workFlowRespnse));
        var response = await _service.GetWorkFlows();

        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(workFlowRespnse);
    }


    [Fact]
    public async Task GetWorkflowsDbWorkflowsAreNull()
    {
        var workFlowRespnse = new List<WorkflowsResponse>()
        {
            AutoFaker.Generate<WorkflowsResponse>()
        };
        _workflowRepository.GetAll().ReturnsNull();
        _http.Expect(HttpMethod.Get, "https://testuri/workflows").Respond(HttpStatusCode.OK, JsonContent.Create(workFlowRespnse));
        var response = await _service.GetWorkFlows();

        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(workFlowRespnse);
    }

    [Fact]
    public async Task GetWorkflowsNoDbWorkflows()
    {
        var workFlowRespnse = new List<WorkflowsResponse>()
        {
            AutoFaker.Generate<WorkflowsResponse>()
        };
        _workflowRepository.GetAll().Returns(new List<Workflow>());
        _http.Expect(HttpMethod.Get, "https://testuri/workflows").Respond(HttpStatusCode.OK, JsonContent.Create(workFlowRespnse));
        var response = await _service.GetWorkFlows();

        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(workFlowRespnse);
    }

    [Fact]
    public async Task GetWorkflowsBadRequestResponse()
    {
        _workflowRepository.GetAll().ReturnsNull();
        _http.Expect(HttpMethod.Get, "https://testuri/workflows").Respond(HttpStatusCode.BadRequest);
        var response = await _service.GetWorkFlows();

        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(new List<Workflow>());
    }

    [Fact]
    public async Task RunWorkflowSuccessResponse()
    {
        _workflowRepository.GetAll().ReturnsNull();
        _http.Expect(HttpMethod.Post, "https://testuri/workflows/1/run").Respond(HttpStatusCode.OK);
        var response = await _service.RunWorkflow(1);

        response.Should().Be(200);
    }
}
