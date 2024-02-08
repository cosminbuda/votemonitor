﻿using GetEndpoint = Vote.Monitor.Api.Feature.UserPreferences.Get.Endpoint;
using GetRequest = Vote.Monitor.Api.Feature.UserPreferences.Get.Request;

namespace Vote.Monitor.Api.IntegrationTests.UserPreferences;
public class GetEndpointTests : IClassFixture<HttpServerFixture>
{
    public HttpServerFixture Fixture { get; }

    public GetEndpointTests(HttpServerFixture fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        Fixture.OutputHelper = outputHelper;
    }


    [Fact]
    public async Task Should_ReturnsUserPreferences_WhenUserExists()
    {
        // Arrange
        GetRequest getRequest = new GetRequest();

        var admin = Fixture.PlatformAdmin.DefaultRequestHeaders.Authorization;

        // Act
        var (getResponse, userPreferences) = await Fixture.PlatformAdmin.GETAsync<GetEndpoint, GetRequest, Dictionary<string, string>>(getRequest);
        // Assert
        getResponse.IsSuccessStatusCode.Should().BeTrue();
        userPreferences.Should().NotBeNull();
    }
}
