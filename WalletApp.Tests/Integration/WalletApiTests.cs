using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using WalletApp.Domain.Entities;

namespace WalletApp.Tests.Integration
{
    public class WalletApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public WalletApiTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();

            var token = GenerateJwt();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }


        private string GenerateJwt()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("uZs9kA1exC6zvMBXyxrXtYH3p3c+N1L7Q2Jq+UkNTh4="));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: new[] { new Claim(ClaimTypes.Name, "TestUser") },
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [Fact]
        public async Task CreateWallet_ThenGetById_ShouldReturnWallet()
        {
            var token = GenerateJwt();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var wallet = new Wallet
            {
                Name = "Mariela",
                DocumentId = "1234567890",
                Balance = 100
            };

            var postResponse = await _client.PostAsJsonAsync("/api/wallets", wallet);
            if (!postResponse.IsSuccessStatusCode)
            {
                var errorContent = await postResponse.Content.ReadAsStringAsync();
                throw new Exception($"Error {postResponse.StatusCode}: {errorContent}");
            }

            var created = await postResponse.Content.ReadFromJsonAsync<Wallet>();

            var getResponse = await _client.GetAsync($"/api/wallets/{created!.Id}");
            getResponse.EnsureSuccessStatusCode();

            var retrieved = await getResponse.Content.ReadFromJsonAsync<Wallet>();

            retrieved.Should().NotBeNull();
            retrieved!.Name.Should().Be("Mariela");
            retrieved.Balance.Should().Be(100);
        }
    }
}
