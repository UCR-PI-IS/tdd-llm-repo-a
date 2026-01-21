using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;
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

        [Test(Description = "Returns HTTP 200 with components list for valid learning space")]
        public async Task GetComponents_ValidLearningSpace_ReturnsComponents()
        {
            var response = await client.GetAsync("/learning-spaces/1/components");
            Assert.AreEqual(200, (int)response.StatusCode);
            var components = await response.Content.ReadAsAsync<List<LearningComponentDto>>();
            Assert.IsTrue(components.Count > 0);
        }

        [Test(Description = "Returns HTTP 200 with empty list when learning space has no components")]
        public async Task GetComponents_ValidLearningSpaceNoComponents_ReturnsEmptyList()
        {
            var response = await client.GetAsync("/learning-spaces/2/components");
            Assert.AreEqual(200, (int)response.StatusCode);
            var components = await response.Content.ReadAsAsync<List<LearningComponentDto>>();
            Assert.IsNotNull(components);
            Assert.AreEqual(0, components.Count);
        }

        [Test(Description = "Returns error (404) with message when learning space ID is invalid")]
        public async Task GetComponents_InvalidLearningSpace_ReturnsError()
        {
            var response = await client.GetAsync("/learning-spaces/999/components");
            Assert.AreEqual(404, (int)response.StatusCode);
            var error = await response.Content.ReadAsAsync<ErrorResponse>();
            Assert.IsFalse(string.IsNullOrEmpty(error.Message));
        }
    }
}

// Minimal mock class for TestApiClient
class TestApiClient
{
    public Task<HttpResponseMessage> GetAsync(string url)
    {
        throw new NotImplementedException("TestApiClient mock should be implemented or replaced with a real test client.");
    }
}

// Minimal mock classes for HttpResponseMessage and related
class HttpResponseMessage
{
    public int StatusCode { get; set; }
    public HttpContent Content { get; set; }
}

class HttpContent
{
    public Task<T> ReadAsAsync<T>() => throw new NotImplementedException();
}

class ErrorResponse
{
    public string Message { get; set; }
}
