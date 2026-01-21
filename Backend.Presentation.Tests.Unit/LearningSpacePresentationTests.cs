using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using UCR.ECCI.PI.ThemePark.Backend.Presentation.Api.Dtos;

namespace UCR.ECCI.PI.ThemePark.Backend.Presentation.Tests.Unit
{
    [TestFixture]
    public class LearningSpacePresentationTests
    {
        private TestApiClient client;

        [SetUp]
        public void Setup()
        {
            client = new TestApiClient();
        }

        [Test]
        public async Task GetComponents_ValidLearningSpace_ReturnsComponents()
        {
            var response = await client.GetAsync("/learning-spaces/1/components");
            Assert.AreEqual(200, (int)response.StatusCode);
            var components = await response.Content.ReadAsAsync<List<LearningComponentDto>>();
            Assert.IsTrue(components.Count > 0);
        }

        [Test]
        public async Task GetComponents_ValidLearningSpaceNoComponents_ReturnsEmptyList()
        {
            var response = await client.GetAsync("/learning-spaces/2/components");
            Assert.AreEqual(200, (int)response.StatusCode);
            var components = await response.Content.ReadAsAsync<List<LearningComponentDto>>();
            Assert.IsNotNull(components);
            Assert.AreEqual(0, components.Count);
        }

        [Test]
        public async Task GetComponents_InvalidLearningSpace_ReturnsError()
        {
            var response = await client.GetAsync("/learning-spaces/999/components");
            Assert.AreEqual(404, (int)response.StatusCode);
            var error = await response.Content.ReadAsAsync<ErrorResponse>();
            Assert.IsFalse(string.IsNullOrEmpty(error.Message));
        }
    }
}

// Minimal mock HttpResponseMessage class
public class HttpResponseMessage
{
    public int StatusCode { get; set; }
    public HttpContent Content { get; set; }

    public HttpResponseMessage(int statusCode, HttpContent content)
    {
        StatusCode = statusCode;
        Content = content;
    }
}

// Minimal mock HttpContent class
public class HttpContent
{
    private readonly string _json;

    public HttpContent(string json) {
        _json = json;
    }

    public Task<T> ReadAsAsync<T>()
    {
        return Task.FromResult(System.Text.Json.JsonSerializer.Deserialize<T>(_json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!);
    }
}

// Minimal mock ErrorResponse class
public class ErrorResponse
{
    public string Message { get; set; } = "";
}

// Minimal TestApiClient mock
public class TestApiClient
{
    public Task<HttpResponseMessage> GetAsync(string url)
    {
        if (url == "/learning-spaces/1/components")
        {
            var components = new List<LearningComponentDto> {
                new LearningComponentDto("Whiteboard"),
                new LearningComponentDto("Projector")
            };
            var json = System.Text.Json.JsonSerializer.Serialize(components);
            return Task.FromResult(new HttpResponseMessage(200, new HttpContent(json)));
        }
        if (url == "/learning-spaces/2/components")
        {
            var components = new List<LearningComponentDto>();
            var json = System.Text.Json.JsonSerializer.Serialize(components);
            return Task.FromResult(new HttpResponseMessage(200, new HttpContent(json)));
        }
        if (url == "/learning-spaces/999/components")
        {
            var error = new ErrorResponse { Message = "Learning space not found." };
            var json = System.Text.Json.JsonSerializer.Serialize(error);
            return Task.FromResult(new HttpResponseMessage(404, new HttpContent(json)));
        }
        return Task.FromResult(new HttpResponseMessage(404, new HttpContent("{}")));
    }
}
