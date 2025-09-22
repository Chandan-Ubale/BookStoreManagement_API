using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStoreAPI.IntegrationTests
{
    public class BooksApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public BooksApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_AllBooks_ReturnsOk()
        {
            var res = await _client.GetAsync("/Books");
            // If no data exists you might get 200 with empty array — just assert success
            res.EnsureSuccessStatusCode();
            var body = await res.Content.ReadAsStringAsync();
            Assert.NotNull(body);
        }
    }
}
