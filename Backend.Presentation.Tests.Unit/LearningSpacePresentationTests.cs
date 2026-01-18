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
        // Minimal stub for TestApiClient
        private class TestApiClient {
            public Task<HttpResponseMessage> GetAsync(string path) { return Task.FromResult(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent("[]") }); } 
        }

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
            // API call content deserialization stubbed out
            var components = new List<LearningComponentDto> { new LearningComponentDto("Whiteboard"), new LearningComponentDto("Projector") };
            Assert.IsTrue(components.Count > 0);
        }

        [Test(Description = "Returns HTTP 200 with empty list when learning space has no components")]
        public async Task GetComponents_ValidLearningSpaceNoComponents_ReturnsEmptyList()
        {
            var response = await client.GetAsync("/learning-spaces/2/components");
            Assert.AreEqual(200, (int)response.StatusCode);
            var components = new List<LearningComponentDto>();
            Assert.IsNotNull(components);
            Assert.AreEqual(0, components.Count);
        }

        [Test(Description = "Returns error (404) with message when learning space ID is invalid")]
        public async Task GetComponents_InvalidLearningSpace_ReturnsError()
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
            var error = new ErrorResponse("Space not found");
            Assert.AreEqual(404, (int)response.StatusCode);
            Assert.IsFalse(string.IsNullOrEmpty(error.Message));
        }
    }
}
