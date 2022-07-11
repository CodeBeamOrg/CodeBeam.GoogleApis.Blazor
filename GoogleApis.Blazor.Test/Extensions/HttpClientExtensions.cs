using System.Net;
using GoogleApis.Blazor.Extensions;
using Moq.Contrib.HttpClient;

namespace GoogleApis.Blazor.Test.Extensions;

public class HttpClientExtensionsTests
{
    private static Mock<HttpMessageHandler> GetMockHttpMessageHandler()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(HttpMethod.Get, "https://www.helloworld.com")
            .ReturnsResponse(HttpStatusCode.OK, "");
        return handler;
    }
    
    [Fact]
    public async Task GetWithQueryStringsAsync__WhenQueryStringsParam_Null__Returns_ArgumentNullException()
    {
        var handler = GetMockHttpMessageHandler();
        
        var client = handler.CreateClient();
        
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetWithQueryStringsAsync("https://www.helloworld.com"));
    }
    
    [Fact]
    public async Task GetWithQueryStringsAsync__WhenQueryStringsParam_Empty__Returns_ArgumentNullException()
    {
        var handler = GetMockHttpMessageHandler();
        
        var client = handler.CreateClient();
        
        await Assert.ThrowsAsync<ArgumentNullException>(() => client.GetWithQueryStringsAsync("https://www.helloworld.com", Array.Empty<string>()));
    }
        
    [Fact]
    public async Task GetWithQueryStringsAsync__WhenQueryStringsParam_NotEven__Returns_ArgumentOutOfRangeException()
    {
        var handler = GetMockHttpMessageHandler();
        
        var client = handler.CreateClient();
        
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => client.GetWithQueryStringsAsync("https://www.helloworld.com", new []{ "", "", ""}));
    }
            
    [Fact]
    public async Task GetWithQueryStringsAsync__WhenQueryStringsParam_HasNullKey__Returns_ArgumentException()
    {
        var handler = GetMockHttpMessageHandler();
        
        var client = handler.CreateClient();
        
        await Assert.ThrowsAsync<ArgumentException>(() => client.GetWithQueryStringsAsync("https://www.helloworld.com", new []{ null, "something"}));
    }
                
    [Fact]
    public async Task GetWithQueryStringsAsync__ReturnsExpectedResult()
    {
        var handler = new Mock<HttpMessageHandler>();
        handler.SetupRequest(HttpMethod.Get, "https://www.helloworld.com?fruit=apple&size=large")
            .ReturnsResponse(HttpStatusCode.OK, "hello_world");
        var client = new HttpClient(handler.Object);
        
        var response = await client.GetWithQueryStringsAsync("https://www.helloworld.com", new[]
        {
            "fruit", "apple",
            "size", "large",
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal("hello_world", body);
    }
}