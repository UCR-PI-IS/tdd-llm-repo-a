using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Backend.Presentation.Responses;

namespace Backend.Presentation.Tests.Unit.Handlers.ComponentsManagement
{
    [TestFixture]
    public class GetLearningComponentHandlerTests
    {
        private readonly Guid validId = Guid.NewGuid();
        private readonly Guid validIdNoComponents = Guid.NewGuid();
        private readonly Guid invalidId = Guid.Empty;

        [Test(Description = "GET returns HTTP 200 and components list for valid learning space with components")]
        public async Task GetLearningComponents_ValidId_ReturnsComponents()
        {
            var client = new HttpClient(); // Assume configured test client
            var response = await client.GetAsync($"/api/learningspaces/{validId}/components");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var components = await response.Content.ReadAsAsync<System.Collections.Generic.List<object>>();
            Assert.IsNotEmpty(components);
        }

        [Test(Description = "GET returns HTTP 200 and empty list for valid learning space with no components")]
        public async Task GetLearningComponents_ValidIdNoComponents_ReturnsEmptyList()
        {
            var client = new HttpClient(); // Assume configured test client
            var response = await client.GetAsync($"/api/learningspaces/{validIdNoComponents}/components");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var components = await response.Content.ReadAsAsync<System.Collections.Generic.List<object>>();
            Assert.IsEmpty(components);
        }

        [Test(Description = "GET returns HTTP 404 and error message for invalid learning space id")]
        public async Task GetLearningComponents_InvalidId_ReturnsNotFound()
        {
            var client = new HttpClient(); // Assume configured test client
            var response = await client.GetAsync($"/api/learningspaces/{invalidId}/components");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
            var error = await response.Content.ReadAsAsync<ErrorResponse>();
            Assert.IsNotNull(error.Message);
        }
    }
}
