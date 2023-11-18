using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using ToDoAPI.Models;
using Xunit.Priority;

namespace ToDoAPI.Test
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class AuthenticationControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private static IConfiguration _configuration;
        private static Admin _adminSeed;
        private static User _userSeed;
        private readonly HttpClient _httpClient;

        public AuthenticationControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateDefaultClient();
            SetupClient();
        }

        [Fact, Priority(1)]
        public async void Create_Admin_And_User_400()
        {
            _adminSeed = new Admin
            {
                Username = $"Admin{DateTime.Now:yyyyMMddHHmmss}",
                Password = "",
                Email = "seed@gmail.com"
            };
            var response1 = await _httpClient.PostAsync("api/Authentication/RegisterAdmin",
                new StringContent(JsonConvert.SerializeObject(_adminSeed), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            _userSeed = new User
            {
                Username = $"User{DateTime.Now:yyyyMMddHHmmss}",
                Password = "",
                Email = "seed@gmail.com"
            };
            var response2 = await _httpClient.PostAsync("api/Authentication/Register",
                new StringContent(JsonConvert.SerializeObject(_userSeed), Encoding.UTF8, "application/json"));
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact, Priority(2)]
        public async void Create_Admin_And_User_500()
        {
            var httpClient = _factory.CreateDefaultClient();
            _adminSeed = new Admin
            {
                Username = $"Admin{DateTime.Now:yyyyMMddHHmmss}",
                Password = "Password123",
                Email = "seed@gmail.com"
            };
            var response1 = await httpClient.PostAsync("api/Authentication/RegisterAdmin",
                new StringContent(JsonConvert.SerializeObject(_adminSeed), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
            var response1String = await response1.Content.ReadAsStringAsync();
            response1String.Should().Contain("Passwords must have at least one non alphanumeric character");

            _userSeed = new User
            {
                Username = $"User{DateTime.Now:yyyyMMddHHmmss}",
                Password = "Password123",
                Email = "seed@gmail.com"
            };
            var response2 = await httpClient.PostAsync("api/Authentication/Register",
                new StringContent(JsonConvert.SerializeObject(_userSeed), Encoding.UTF8, "application/json"));
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);
            var response2String = await response2.Content.ReadAsStringAsync();
            response2String.Should().Contain("Passwords must have at least one non alphanumeric character");
        }

        [Fact, Priority(3)]
        public async void Create_Admin_Login_200()
        {
            _adminSeed = new Admin
            {
                Username = $"Admin{DateTime.Now:yyyyMMddHHmmss}",
                Password = "Password@123",
                Email = "admin@gmail.com"
            };
            var response1 = await _httpClient.PostAsync("api/Authentication/RegisterAdmin",
                new StringContent(JsonConvert.SerializeObject(_adminSeed), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response1String = await response1.Content.ReadAsStringAsync();

            var login = new LoginModel
            {
                Username = _adminSeed.Username,
                Password = _adminSeed.Password
            };
            var response2 = await _httpClient.PostAsync("api/Authentication/Login",
                new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json"));
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response2String = await response2.Content.ReadAsStringAsync();
            response2String.Should().NotBeNullOrWhiteSpace();
            var response2Content = JsonConvert.DeserializeObject<LoginToken>(response2String);
            response2Content.Should().NotBeNull();
            response2Content.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact, Priority(4)]
        public async void Admin_Login_401()
        {
            var login = new LoginModel
            {
                Username = _adminSeed.Username,
                Password = "Password@456"
            };
            var response2 = await _httpClient.PostAsync("api/Authentication/Login",
                new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json"));
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var response2String = await response2.Content.ReadAsStringAsync();
            response2String.Should().NotBeNullOrWhiteSpace();
        }

        [Fact, Priority(5)]
        public async void Create_User_Login_200()
        {
            _userSeed = new User
            {
                Username = $"User{DateTime.Now:yyyyMMddHHmmss}",
                Password = "Password@123",
                Email = "user@gmail.com"
            };
            var response1 = await _httpClient.PostAsync("api/Authentication/Register",
                new StringContent(JsonConvert.SerializeObject(_userSeed), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response1String = await response1.Content.ReadAsStringAsync();

            var login = new LoginModel
            {
                Username = _userSeed.Username,
                Password = _userSeed.Password
            };
            var response2 = await _httpClient.PostAsync("api/Authentication/Login",
                new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json"));
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response2String = await response2.Content.ReadAsStringAsync();
            response2String.Should().NotBeNullOrWhiteSpace();
            var response2Content = JsonConvert.DeserializeObject<LoginToken>(response2String);
            response2Content.Should().NotBeNull();
            response2Content.Token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact, Priority(6)]
        public async void User_Login_401()
        {
            var login = new LoginModel
            {
                Username = _userSeed.Username,
                Password = "Password@456"
            };
            var response2 = await _httpClient.PostAsync("api/Authentication/Login",
                new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json"));
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var response2String = await response2.Content.ReadAsStringAsync();
            response2String.Should().NotBeNullOrWhiteSpace();
        }

        private void SetupClient()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:ValidAudience", @"http://localhost:4200"},
                {"Jwt:ValidIssuer", @"http://localhost:5000"},
                {"Jwt:Secret", "JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"},
                {"Jwt:ExpiryMinutes", "180"},
            };

            _configuration = new ConfigurationBuilder()
              .AddInMemoryCollection(inMemorySettings)
              .Build();
        }

        public class Admin : User
        {
        }

        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }
    }
}
