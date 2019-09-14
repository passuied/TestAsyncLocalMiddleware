using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace TestAsyncLocalMiddleware.Tests
{
    public class ReproduceIssueWithAsyncLocal
    {
        [Fact]
        public async Task When2SeparateRequests_AsyncLocalShouldNotRetainFirstRequestValues()
        {
            using (var server = new TestServer(
                WebHost.CreateDefaultBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()))
            {
                var client = server.CreateClient();
                var request = new HttpRequestMessage();
                request.Headers.Add("x-corp-id", "1");
                request.Headers.Add("x-user-id", "5");
                request.Headers.Add("x-auth-corp-id", "10");
                request.Headers.Add("x-auth-user-id", "50");
                request.RequestUri = new Uri("/api/values", UriKind.Relative);
                request.Method = HttpMethod.Get;

                var resp = await client.SendAsync(request);

                resp.IsSuccessStatusCode.Should().BeTrue();

                // Interestingly when running the same test with TestHost, the same behavior doesn't occur...
                var request2 = new HttpRequestMessage();
                request2.Headers.Add("x-corp-id", "2");
                request2.Headers.Add("x-user-id", "8");
                request2.Headers.Add("x-auth-corp-id", "20");
                request2.Headers.Add("x-auth-user-id", "80");
                request2.RequestUri = new Uri("/api/values", UriKind.Relative);
                request2.Method = HttpMethod.Get;

                var resp2 = await client.SendAsync(request2);

                resp2.IsSuccessStatusCode.Should().BeTrue();


            }
        }
    }
}
