using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using ToDoAPI.BLL.Models;
using ToDoAPI.Core.Constants;
using ToDoAPI.Core.Enums;
using ToDoAPI.Models;
using Xunit.Priority;
using static ToDoAPI.Test.AuthenticationControllerTest;
using Response = ToDoAPI.Models.Response;

namespace ToDoAPI.Test
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class ToDoControllerTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private static IConfiguration _configuration;
        private static Admin _adminSeed;
        private static User _userSeed;
        private static string _adminToken;
        private static string _userToken;
        private readonly HttpClient _adminHttpClient;
        private readonly HttpClient _userHttpClient;

        public ToDoControllerTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _adminHttpClient = _factory.CreateDefaultClient();
            _userHttpClient = _factory.CreateDefaultClient();
            SetupClient();
        }

        [Fact, Priority(1)]
        public async void SeedAdminAndUserData()
        {
            // Admin
            _adminSeed = new Admin
            {
                Username = $"Admin{DateTime.Now:yyyyMMddHHmmssfff}",
                Password = "Password@123",
                Email = "admin@gmail.com"
            };
            var response1 = await _adminHttpClient.PostAsync("api/Authentication/RegisterAdmin",
                new StringContent(JsonConvert.SerializeObject(_adminSeed), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response1String = await response1.Content.ReadAsStringAsync();

            var adminLogin = new LoginModel
            {
                Username = _adminSeed.Username,
                Password = _adminSeed.Password
            };
            var response2 = await _adminHttpClient.PostAsync("api/Authentication/Login",
                new StringContent(JsonConvert.SerializeObject(adminLogin), Encoding.UTF8, "application/json"));
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response2String = await response2.Content.ReadAsStringAsync();
            response2String.Should().NotBeNullOrWhiteSpace();

            var response2Content = JsonConvert.DeserializeObject<LoginToken>(response2String);
            response2Content.Should().NotBeNull();
            response2Content.Token.Should().NotBeNullOrWhiteSpace();
            _adminToken = response2Content.Token;
            _adminHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_adminToken}");

            // User
            _userSeed = new User
            {
                Username = $"User{DateTime.Now:yyyyMMddHHmmssfff}",
                Password = "Password@123",
                Email = "user@gmail.com"
            };
            var response3 = await _userHttpClient.PostAsync("api/Authentication/Register",
                new StringContent(JsonConvert.SerializeObject(_userSeed), Encoding.UTF8, "application/json"));
            response3.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response3String = await response3.Content.ReadAsStringAsync();

            var userLogin = new LoginModel
            {
                Username = _userSeed.Username,
                Password = _userSeed.Password
            };
            var response4 = await _userHttpClient.PostAsync("api/Authentication/Login",
                new StringContent(JsonConvert.SerializeObject(userLogin), Encoding.UTF8, "application/json"));
            response4.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response4String = await response4.Content.ReadAsStringAsync();
            response4String.Should().NotBeNullOrWhiteSpace();

            var response4Content = JsonConvert.DeserializeObject<LoginToken>(response4String);
            response4Content.Should().NotBeNull();
            response4Content.Token.Should().NotBeNullOrWhiteSpace();
            _userToken = response4Content.Token;
            _userHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_userToken}");
        }

        [Fact, Priority(2)]
        public async void Admin_CreateToDoItem_200()
        {
            var current = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var toDoItem1 = new CreateToDoItemWithTags
            {
                Name = $"ToDo {current}",
                Description = $"ToDo Description {current}",
                DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                Status = (int)ToDoItemStatus.NotStarted,
                Priority = 1
            };
            _adminHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_adminToken}");

            var response1 = await _adminHttpClient.PostAsync("api/ToDo/CreateToDoItem",
                new StringContent(JsonConvert.SerializeObject(toDoItem1), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response1String = await response1.Content.ReadAsStringAsync();
            var response1Content = JsonConvert.DeserializeObject<Response>(response1String);
            response1Content.Status.Should().NotBeNullOrWhiteSpace();
            response1Content.Status.Should().Be(StatusMessage.Success);

            var search = new GetToDoItemFilters
            {
                Name = toDoItem1.Name,
                Status = new[] { toDoItem1.Status.Value },
                DueDateFrom = DateTime.Now.ToString("yyyy-MM-dd"),
                DueDateTo = DateTime.Now.AddDays(10).ToString("yyyy-MM-dd")
            };
            var response2 = await _adminHttpClient.GetAsync($"api/ToDo/GetToDoItemWithTags?Name={toDoItem1.Name}" +
                $"&Status={toDoItem1.Status.Value}&DueDateFrom={search.DueDateFrom}&DueDateTo={search.DueDateTo}");
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response2String = await response2.Content.ReadAsStringAsync();
            var response2Content = JsonConvert.DeserializeObject<IEnumerable<ToDoItemWithTags>>(response2String);
            response2Content.Should().NotBeNull();
            response2Content.Count().Should().Be(1);

            var checkToDo = response2Content.First();
            checkToDo.Name.Should().Be(toDoItem1.Name);
        }

        [Fact, Priority(3)]
        public async void Admin_CreateToDoItem_400_401()
        {
            var current = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var toDoItem1 = new CreateToDoItemWithTags
            {
                Name = $"ToDo {current}",
                Description = null,
                DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                Status = (int)ToDoItemStatus.NotStarted,
                Priority = 1,
                ToDoItemTagList = new List<ToDoAPI.Models.ToDoItemTag> { new ToDoAPI.Models.ToDoItemTag
                {
                    TagKey = $"ToDo Tag {current}",
                    TagValue = $"ToDo Description {current}"
                }}
            };

            var response1 = await _adminHttpClient.PostAsync("api/ToDo/CreateToDoItem",
                new StringContent(JsonConvert.SerializeObject(toDoItem1), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);

            _adminHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_adminToken}");

            response1 = await _adminHttpClient.PostAsync("api/ToDo/CreateToDoItem",
                new StringContent(JsonConvert.SerializeObject(toDoItem1), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var response1String = await response1.Content.ReadAsStringAsync();
        }

        [Fact, Priority(4)]
        public async void Admin_Create_Update_ToDoItemWithTags_200()
        {
            var current = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var toDoItem1 = new CreateToDoItemWithTags
            {
                Name = $"ToDo {current}",
                Description = $"ToDo Description {current}",
                DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                Status = (int)ToDoItemStatus.NotStarted,
                Priority = 1,
                ToDoItemTagList = new List<ToDoAPI.Models.ToDoItemTag> { new ToDoAPI.Models.ToDoItemTag
                {
                    TagKey = $"ToDo Tag {current}",
                    TagValue = $"ToDo Description {current}"
                }}
            };
            _adminHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_adminToken}");

            var response1 = await _adminHttpClient.PostAsync("api/ToDo/CreateToDoItem",
                new StringContent(JsonConvert.SerializeObject(toDoItem1), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response1String = await response1.Content.ReadAsStringAsync();
            var response1Content = JsonConvert.DeserializeObject<Response>(response1String);
            response1Content.Status.Should().NotBeNullOrWhiteSpace();
            response1Content.Status.Should().Be(StatusMessage.Success);

            var search = new GetToDoItemFilters
            {
                Name = toDoItem1.Name,
                Status = new[] { toDoItem1.Status.Value },
                DueDateFrom = DateTime.Now.ToString("yyyy-MM-dd"),
                DueDateTo = DateTime.Now.AddDays(10).ToString("yyyy-MM-dd")
            };
            var response2 = await _adminHttpClient.GetAsync($"api/ToDo/GetToDoItemWithTags?Name={toDoItem1.Name}" +
                $"&Status={toDoItem1.Status.Value}&DueDateFrom={search.DueDateFrom}&DueDateTo={search.DueDateTo}," +
                $"&Sorting=DueDate|DESC");
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response2String = await response2.Content.ReadAsStringAsync();
            var response2Content = JsonConvert.DeserializeObject<IEnumerable<ToDoItemWithTags>>(response2String);
            response2Content.Should().NotBeNull();
            response2Content.Count().Should().Be(1);
            response2Content.First().ToDoItemTagList.Count().Should().Be(1);

            var update = new ToDoAPI.BLL.Models.UpdateToDoItem
            {
                Id = response2Content.First().Id,
                Name = $"Update ToDo {current}",
                Description = $"Update ToDo Description {current}",
                Status = (int)ToDoItemStatus.InProgress,
                Priority = toDoItem1.Priority
            };
            var response3 = await _adminHttpClient.PatchAsync("api/ToDo/UpdateToDoItem",
                new StringContent(JsonConvert.SerializeObject(update), Encoding.UTF8, "application/json"));
            response3.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response3String = await response3.Content.ReadAsStringAsync();
            var response3Content = JsonConvert.DeserializeObject<Response>(response3String);
            response3Content.Status.Should().NotBeNullOrWhiteSpace();
            response3Content.Status.Should().Be(StatusMessage.Success);

            var updateTag = new ToDoAPI.BLL.Models.ToDoItemTag
            {
                Id = response2Content.First().ToDoItemTagList.First().Id,
                TagKey = $"Update ToDo Tag {current}",
                TagValue = $"Update ToDo Description {current}"
            };
            var response4 = await _adminHttpClient.PatchAsync("api/ToDo/UpdateToDoItemTag",
                new StringContent(JsonConvert.SerializeObject(updateTag), Encoding.UTF8, "application/json"));
            response4.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response4String = await response3.Content.ReadAsStringAsync();
            var response4Content = JsonConvert.DeserializeObject<Response>(response3String);
            response4Content.Status.Should().NotBeNullOrWhiteSpace();
            response4Content.Status.Should().Be(StatusMessage.Success);

            var search2 = new GetToDoItemFilters
            {
                Name = toDoItem1.Name,
                Status = new[] { update.Status.Value },
                DueDateFrom = DateTime.Now.ToString("yyyy-MM-dd"),
                DueDateTo = DateTime.Now.AddDays(10).ToString("yyyy-MM-dd")
            };
            var response5 = await _adminHttpClient.GetAsync($"api/ToDo/GetToDoItemWithTags?Name={update.Name}" +
                $"&Status={update.Status.Value}&DueDateFrom={search2.DueDateFrom}&DueDateTo={search2.DueDateTo}," +
                $"&Sorting=DueDate|DESC");
            response5.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response5String = await response5.Content.ReadAsStringAsync();
            var response5Content = JsonConvert.DeserializeObject<IEnumerable<ToDoItemWithTags>>(response5String);
            response5Content.Should().NotBeNull();
            response5Content.Count().Should().Be(1);

            var updatedToDo = response5Content.First();
            updatedToDo.Name.Should().Be(update.Name);
            updatedToDo.Description.Should().Be(update.Description);
            updatedToDo.Status = updatedToDo.Status;
            updatedToDo.Priority = updatedToDo.Priority;

            var updatedToDoTag = updatedToDo.ToDoItemTagList.First();
            updatedToDoTag.TagKey.Should().Be(updateTag.TagKey);
            updatedToDoTag.TagValue.Should().Be(updateTag.TagValue);
        }

        [Fact, Priority(5)]
        public async void Admin_Create_Delete_ToDoItemWithTags_200()
        {
            var current = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var toDoItem1 = new CreateToDoItemWithTags
            {
                Name = $"ToDo {current}",
                Description = $"ToDo Description {current}",
                DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                Status = (int)ToDoItemStatus.NotStarted,
                Priority = 1,
                ToDoItemTagList = new List<ToDoAPI.Models.ToDoItemTag> { new ToDoAPI.Models.ToDoItemTag
                {
                    TagKey = $"ToDo Tag {current}",
                    TagValue = $"ToDo Description {current}"
                }}
            };
            _adminHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_adminToken}");

            var response1 = await _adminHttpClient.PostAsync("api/ToDo/CreateToDoItem",
                new StringContent(JsonConvert.SerializeObject(toDoItem1), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response1String = await response1.Content.ReadAsStringAsync();
            var response1Content = JsonConvert.DeserializeObject<Response>(response1String);
            response1Content.Status.Should().NotBeNullOrWhiteSpace();
            response1Content.Status.Should().Be(StatusMessage.Success);

            var search = new GetToDoItemFilters
            {
                Name = toDoItem1.Name,
                Status = new[] { toDoItem1.Status.Value },
                DueDateFrom = DateTime.Now.ToString("yyyy-MM-dd"),
                DueDateTo = DateTime.Now.AddDays(10).ToString("yyyy-MM-dd")
            };
            var response2 = await _adminHttpClient.GetAsync($"api/ToDo/GetToDoItemWithTags?Name={toDoItem1.Name}" +
                $"&Status={toDoItem1.Status.Value}&DueDateFrom={search.DueDateFrom}&DueDateTo={search.DueDateTo}," +
                $"&Sorting=DueDate|DESC");
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response2String = await response2.Content.ReadAsStringAsync();
            var response2Content = JsonConvert.DeserializeObject<IEnumerable<ToDoItemWithTags>>(response2String);
            response2Content.Should().NotBeNull();
            response2Content.Count().Should().Be(1);
            response2Content.First().ToDoItemTagList.Count().Should().Be(1);

            var response3 = await _adminHttpClient.DeleteAsync($"api/ToDo/DeleteToDoItem?id={response2Content.First().Id}");
            response3.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response3String = await response3.Content.ReadAsStringAsync();
            var response3Content = JsonConvert.DeserializeObject<Response>(response3String);
            response3Content.Status.Should().NotBeNullOrWhiteSpace();
            response3Content.Status.Should().Be(StatusMessage.Success);

            var response4 = await _adminHttpClient.GetAsync($"api/ToDo/GetToDoItemWithTagsById?id={response2Content.First().Id}");
            response4.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var response4String = await response4.Content.ReadAsStringAsync();
            var response4Content = JsonConvert.DeserializeObject<Response>(response4String);
            response4Content.Should().NotBeNull();
            response4Content.Message.Should().Be(ResponseMessage.ToDoItemDNE);

            var response5 = await _adminHttpClient.GetAsync($"api/ToDo/GetToDoItemTagsById?id={response2Content.First().ToDoItemTagList.First().Id}");
            response5.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var response5String = await response5.Content.ReadAsStringAsync();
            var response5Content = JsonConvert.DeserializeObject<Response>(response5String);
            response5Content.Should().NotBeNull();
            response5Content.Message.Should().Be(ResponseMessage.ToDoItemTagDNE);
        }

        [Fact, Priority(6)]
        public async Task User_Create_ToDoItem_200()
        {
            var current = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var toDoItem1 = new CreateToDoItemWithTags
            {
                Name = $"ToDo {current}",
                Description = $"ToDo Description {current}",
                DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                Status = (int)ToDoItemStatus.NotStarted,
                Priority = 2
            };
            _userHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_userToken}");

            var response1 = await _userHttpClient.PostAsync("api/ToDo/CreateToDoItem",
                new StringContent(JsonConvert.SerializeObject(toDoItem1), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response1String = await response1.Content.ReadAsStringAsync();
            var response1Content = JsonConvert.DeserializeObject<Response>(response1String);
            response1Content.Status.Should().NotBeNullOrWhiteSpace();
            response1Content.Status.Should().Be(StatusMessage.Success);

            var search = new GetToDoItemFilters
            {
                Name = toDoItem1.Name,
                Status = new[] { toDoItem1.Status.Value },
                DueDateFrom = DateTime.Now.ToString("yyyy-MM-dd"),
                DueDateTo = DateTime.Now.AddDays(10).ToString("yyyy-MM-dd")
            };
            var response2 = await _userHttpClient.GetAsync($"api/ToDo/GetToDoItemWithTags?Name={toDoItem1.Name}" +
                $"&Status={toDoItem1.Status.Value}&DueDateFrom={search.DueDateFrom}&DueDateTo={search.DueDateTo}");
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response2String = await response2.Content.ReadAsStringAsync();
            var response2Content = JsonConvert.DeserializeObject<IEnumerable<ToDoItemWithTags>>(response2String);
            response2Content.Should().NotBeNull();
            response2Content.Count().Should().Be(1);

            var checkToDo = response2Content.First();
            checkToDo.Name.Should().Be(toDoItem1.Name);
        }

        [Fact, Priority(7)]
        public async void User_CreateToDoItem_400_401()
        {
            var current = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var toDoItem1 = new CreateToDoItemWithTags
            {
                Name = $"ToDo {current}",
                Description = null,
                DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                Status = (int)ToDoItemStatus.NotStarted,
                Priority = 1,
                ToDoItemTagList = new List<ToDoAPI.Models.ToDoItemTag> { new ToDoAPI.Models.ToDoItemTag
                {
                    TagKey = $"ToDo Tag {current}",
                    TagValue = $"ToDo Description {current}"
                }}
            };

            var response1 = await _userHttpClient.PostAsync("api/ToDo/CreateToDoItem",
                new StringContent(JsonConvert.SerializeObject(toDoItem1), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);

            _userHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_userToken}");

            response1 = await _userHttpClient.PostAsync("api/ToDo/CreateToDoItem",
                new StringContent(JsonConvert.SerializeObject(toDoItem1), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var response1String = await response1.Content.ReadAsStringAsync();
        }

        [Fact, Priority(8)]
        public async void User_Create_Update_ToDoItemWithTags_200()
        {
            var current = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var toDoItem1 = new CreateToDoItemWithTags
            {
                Name = $"ToDo {current}",
                Description = $"ToDo Description {current}",
                DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                Status = (int)ToDoItemStatus.NotStarted,
                Priority = 1,
                ToDoItemTagList = new List<ToDoAPI.Models.ToDoItemTag> { new ToDoAPI.Models.ToDoItemTag
                {
                    TagKey = $"ToDo Tag {current}",
                    TagValue = $"ToDo Description {current}"
                }}
            };
            _userHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_userToken}");

            var response1 = await _userHttpClient.PostAsync("api/ToDo/CreateToDoItem",
                new StringContent(JsonConvert.SerializeObject(toDoItem1), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response1String = await response1.Content.ReadAsStringAsync();
            var response1Content = JsonConvert.DeserializeObject<Response>(response1String);
            response1Content.Status.Should().NotBeNullOrWhiteSpace();
            response1Content.Status.Should().Be(StatusMessage.Success);

            var search = new GetToDoItemFilters
            {
                Name = toDoItem1.Name,
                Status = new[] { toDoItem1.Status.Value },
                DueDateFrom = DateTime.Now.ToString("yyyy-MM-dd"),
                DueDateTo = DateTime.Now.AddDays(10).ToString("yyyy-MM-dd")
            };
            var response2 = await _userHttpClient.GetAsync($"api/ToDo/GetToDoItemWithTags?Name={toDoItem1.Name}" +
                $"&Status={toDoItem1.Status.Value}&DueDateFrom={search.DueDateFrom}&DueDateTo={search.DueDateTo}," +
                $"&Sorting=DueDate|DESC");
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response2String = await response2.Content.ReadAsStringAsync();
            var response2Content = JsonConvert.DeserializeObject<IEnumerable<ToDoItemWithTags>>(response2String);
            response2Content.Should().NotBeNull();
            response2Content.Count().Should().Be(1);
            response2Content.First().ToDoItemTagList.Count().Should().Be(1);

            var update = new ToDoAPI.BLL.Models.UpdateToDoItem
            {
                Id = response2Content.First().Id,
                Name = $"Update ToDo {current}",
                Description = $"Update ToDo Description {current}",
                Status = (int)ToDoItemStatus.InProgress,
                Priority = toDoItem1.Priority
            };
            var response3 = await _userHttpClient.PatchAsync("api/ToDo/UpdateToDoItem",
                new StringContent(JsonConvert.SerializeObject(update), Encoding.UTF8, "application/json"));
            response3.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response3String = await response3.Content.ReadAsStringAsync();
            var response3Content = JsonConvert.DeserializeObject<Response>(response3String);
            response3Content.Status.Should().NotBeNullOrWhiteSpace();
            response3Content.Status.Should().Be(StatusMessage.Success);

            var updateTag = new ToDoAPI.BLL.Models.ToDoItemTag
            {
                Id = response2Content.First().ToDoItemTagList.First().Id,
                TagKey = $"Update ToDo Tag {current}",
                TagValue = $"Update ToDo Description {current}"
            };
            var response4 = await _userHttpClient.PatchAsync("api/ToDo/UpdateToDoItemTag",
                new StringContent(JsonConvert.SerializeObject(updateTag), Encoding.UTF8, "application/json"));
            response4.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response4String = await response3.Content.ReadAsStringAsync();
            var response4Content = JsonConvert.DeserializeObject<Response>(response3String);
            response4Content.Status.Should().NotBeNullOrWhiteSpace();
            response4Content.Status.Should().Be(StatusMessage.Success);

            var search2 = new GetToDoItemFilters
            {
                Name = toDoItem1.Name,
                Status = new[] { update.Status.Value },
                DueDateFrom = DateTime.Now.ToString("yyyy-MM-dd"),
                DueDateTo = DateTime.Now.AddDays(10).ToString("yyyy-MM-dd")
            };
            var response5 = await _userHttpClient.GetAsync($"api/ToDo/GetToDoItemWithTags?Name={update.Name}" +
                $"&Status={update.Status.Value}&DueDateFrom={search2.DueDateFrom}&DueDateTo={search2.DueDateTo}," +
                $"&Sorting=DueDate|DESC");
            response5.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response5String = await response5.Content.ReadAsStringAsync();
            var response5Content = JsonConvert.DeserializeObject<IEnumerable<ToDoItemWithTags>>(response5String);
            response5Content.Should().NotBeNull();
            response5Content.Count().Should().Be(1);

            var updatedToDo = response5Content.First();
            updatedToDo.Name.Should().Be(update.Name);
            updatedToDo.Description.Should().Be(update.Description);
            updatedToDo.Status = updatedToDo.Status;
            updatedToDo.Priority = updatedToDo.Priority;

            var updatedToDoTag = updatedToDo.ToDoItemTagList.First();
            updatedToDoTag.TagKey.Should().Be(updateTag.TagKey);
            updatedToDoTag.TagValue.Should().Be(updateTag.TagValue);
        }

        [Fact, Priority(9)]
        public async void User_Create_ToDoItemWithTags_200_Delete_401()
        {
            var current = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var toDoItem1 = new CreateToDoItemWithTags
            {
                Name = $"ToDo {current}",
                Description = $"ToDo Description {current}",
                DueDate = DateTime.Now.AddDays(5).ToString("yyyy-MM-dd"),
                Status = (int)ToDoItemStatus.NotStarted,
                Priority = 1,
                ToDoItemTagList = new List<ToDoAPI.Models.ToDoItemTag> { new ToDoAPI.Models.ToDoItemTag
                {
                    TagKey = $"ToDo Tag {current}",
                    TagValue = $"ToDo Description {current}"
                }}
            };
            _userHttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_userToken}");

            var response1 = await _userHttpClient.PostAsync("api/ToDo/CreateToDoItem",
                new StringContent(JsonConvert.SerializeObject(toDoItem1), Encoding.UTF8, "application/json"));
            response1.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response1String = await response1.Content.ReadAsStringAsync();
            var response1Content = JsonConvert.DeserializeObject<Response>(response1String);
            response1Content.Status.Should().NotBeNullOrWhiteSpace();
            response1Content.Status.Should().Be(StatusMessage.Success);

            var search = new GetToDoItemFilters
            {
                Name = toDoItem1.Name,
                Status = new[] { toDoItem1.Status.Value },
                DueDateFrom = DateTime.Now.ToString("yyyy-MM-dd"),
                DueDateTo = DateTime.Now.AddDays(10).ToString("yyyy-MM-dd")
            };
            var response2 = await _userHttpClient.GetAsync($"api/ToDo/GetToDoItemWithTags?Name={toDoItem1.Name}" +
                $"&Status={toDoItem1.Status.Value}&DueDateFrom={search.DueDateFrom}&DueDateTo={search.DueDateTo}," +
                $"&Sorting=DueDate|DESC");
            response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            var response2String = await response2.Content.ReadAsStringAsync();
            var response2Content = JsonConvert.DeserializeObject<IEnumerable<ToDoItemWithTags>>(response2String);
            response2Content.Should().NotBeNull();
            response2Content.Count().Should().Be(1);
            response2Content.First().ToDoItemTagList.Count().Should().Be(1);

            var response3 = await _userHttpClient.DeleteAsync($"api/ToDo/DeleteToDoItem?id={response2Content.First().Id}");
            response3.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
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
    }
}