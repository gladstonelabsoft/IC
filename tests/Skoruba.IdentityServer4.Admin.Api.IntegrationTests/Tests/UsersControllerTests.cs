using FluentAssertions;
using Newtonsoft.Json;
using Skoruba.IdentityServer4.Admin.Api.Dtos.Users;
using Skoruba.IdentityServer4.Admin.Api.IntegrationTests.Common;
using Skoruba.IdentityServer4.Admin.Api.IntegrationTests.Tests.Base;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Skoruba.IdentityServer4.Admin.Api.IntegrationTests.Tests
{
    public class UsersControllerTests : BaseClassFixture
    {
        public UsersControllerTests(TestFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task GetRolesAsAdmin()
        {
            // Arrange
            SetupAdminClaimsViaHeaders();

            // Act
            var response = await Client.GetAsync("api/users");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetRolesWithoutPermissions()
        {
            // Arrange
            Client.DefaultRequestHeaders.Clear();

            // Act
            var response = await Client.GetAsync("api/users");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);

            // The redirect to login
            response.Headers.Location.ToString().Should().Contain(AuthenticationConsts.AccountLoginPage);
        }

        [Fact]
        public async Task PutResetPasswordByEmail_RightCode_ShoudBeOk()
        {
            #region Create User (Only Admin)

            // Arrange
            SetupAdminClaimsViaHeaders();

            var email = "user@email.com";

            var userDto = new UserDto<string>();
            userDto.UserName = email;
            userDto.Email = email;

            var jsonString = JsonConvert.SerializeObject(userDto);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("api/users", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            #endregion

            #region Get Reset Token (Only Admin)

            // Arrange
            var userResetTokenByEmailApiDto = new UserResetTokenByEmailApiDto();
            userResetTokenByEmailApiDto.Email = email;

            jsonString = JsonConvert.SerializeObject(userResetTokenByEmailApiDto);
            httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            response = await Client.PutAsync("api/users/ResetTokenByEmail", httpContent);

            var contentStream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(contentStream);
            using var jsonReader = new JsonTextReader(streamReader);

            JsonSerializer serializer = new JsonSerializer();

            var code = serializer.Deserialize<string>(jsonReader);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            #endregion

            #region Reset Password by Email (Allow Anonymous)

            // Arrange
            Client.DefaultRequestHeaders.Clear();

            var newPassword = "Qu@dr@d1";

            var userResetPasswordByEmailApiDto = new UserResetPasswordByEmailApiDto();
            userResetPasswordByEmailApiDto.Email = email;
            userResetPasswordByEmailApiDto.Password = newPassword;
            userResetPasswordByEmailApiDto.Code = code;

            jsonString = JsonConvert.SerializeObject(userResetPasswordByEmailApiDto);
            httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            response = await Client.PutAsync("api/users/ResetPasswordByEmail", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            #endregion

            #region Change Password by Email (Only Admin)

            // Arrange
            SetupAdminClaimsViaHeaders();

            var currentPassword = newPassword;
            newPassword = "Qu@dr@d2";

            var userChangePasswordByEmailApiDto = new UserChangePasswordByEmailApiDto();
            userChangePasswordByEmailApiDto.Email = email;
            userChangePasswordByEmailApiDto.CurrentPassword = currentPassword;
            userChangePasswordByEmailApiDto.NewPassword = newPassword;

            jsonString = JsonConvert.SerializeObject(userChangePasswordByEmailApiDto);
            httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            response = await Client.PutAsync("api/users/ChangePasswordByEmail", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            #endregion
        }

        [Fact]
        public async Task PutResetPasswordByEmail_WrongCurrentPassword_ShoudBeBadRequest()
        {
            #region Create User (Only Admin)

            // Arrange
            SetupAdminClaimsViaHeaders();

            var email = "user@email.com";

            var userDto = new UserDto<string>();
            userDto.UserName = email;
            userDto.Email = email;

            var jsonString = JsonConvert.SerializeObject(userDto);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("api/users", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            #endregion

            #region Get Reset Token (Only Admin)

            // Arrange
            var userResetTokenByEmailApiDto = new UserResetTokenByEmailApiDto();
            userResetTokenByEmailApiDto.Email = email;

            jsonString = JsonConvert.SerializeObject(userResetTokenByEmailApiDto);
            httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            response = await Client.PutAsync("api/users/ResetTokenByEmail", httpContent);

            var contentStream = await response.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(contentStream);
            using var jsonReader = new JsonTextReader(streamReader);

            JsonSerializer serializer = new JsonSerializer();

            var code = serializer.Deserialize<string>(jsonReader);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            #endregion

            #region Reset Password by Email (Allow Anonymous)

            // Arrange
            Client.DefaultRequestHeaders.Clear();

            var newPassword = "Qu@dr@d1";

            var userResetPasswordByEmailApiDto = new UserResetPasswordByEmailApiDto();
            userResetPasswordByEmailApiDto.Email = email;
            userResetPasswordByEmailApiDto.Password = newPassword;
            userResetPasswordByEmailApiDto.Code = code;

            jsonString = JsonConvert.SerializeObject(userResetPasswordByEmailApiDto);
            httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            response = await Client.PutAsync("api/users/ResetPasswordByEmail", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            #endregion

            #region Change Password by Email (Only Admin)

            // Arrange
            SetupAdminClaimsViaHeaders();

            var currentPassword = "WrongCurrentPassword";
            newPassword = "Qu@dr@d2";

            var userChangePasswordByEmailApiDto = new UserChangePasswordByEmailApiDto();
            userChangePasswordByEmailApiDto.Email = email;
            userChangePasswordByEmailApiDto.CurrentPassword = currentPassword;
            userChangePasswordByEmailApiDto.NewPassword = newPassword;

            jsonString = JsonConvert.SerializeObject(userChangePasswordByEmailApiDto);
            httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            response = await Client.PutAsync("api/users/ChangePasswordByEmail", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            #endregion
        }

        [Fact]
        public async Task PutResetPasswordByEmail_WrongCode_ShoudBeOk()
        {
            #region Create User (Only Admin)

            // Arrange
            SetupAdminClaimsViaHeaders();

            var email = "user@email.com";
            var password = "Qu@dr@d0";
            var code = "code";

            var userDto = new UserDto<string>();
            userDto.UserName = email;
            userDto.Email = email;

            var jsonString = JsonConvert.SerializeObject(userDto);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PostAsync("api/users", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            #endregion

            #region Reset Password by Email (Allow Anonymous)

            // Arrange
            Client.DefaultRequestHeaders.Clear();

            var userResetPasswordByEmailApiDto = new UserResetPasswordByEmailApiDto();
            userResetPasswordByEmailApiDto.Email = email;
            userResetPasswordByEmailApiDto.Password = password;
            userResetPasswordByEmailApiDto.Code = code;

            jsonString = JsonConvert.SerializeObject(userResetPasswordByEmailApiDto);
            httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            response = await Client.PutAsync("api/users/ResetPasswordByEmail", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            #endregion
        }

        [Fact]
        public async Task PutResetPasswordByEmail_UserNotExists_ShoudBeOk()
        {
            #region Reset Password by Email (Allow Anonymous)

            // Arrange
            Client.DefaultRequestHeaders.Clear();

            var email = "user@email.com";
            var password = "Qu@dr@d0";
            var code = "code";

            var userResetPasswordByEmailApiDto = new UserResetPasswordByEmailApiDto();
            userResetPasswordByEmailApiDto.Email = email;
            userResetPasswordByEmailApiDto.Password = password;
            userResetPasswordByEmailApiDto.Code = code;

            var jsonString = JsonConvert.SerializeObject(userResetPasswordByEmailApiDto);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync("api/users/ResetPasswordByEmail", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            #endregion
        }

        [Fact]
        public async Task PutResetPasswordByEmail_WrongJson_ShoudBeBadRequest()
        {
            #region Reset Password by Email (Allow Anonymous)

            // Arrange
            Client.DefaultRequestHeaders.Clear();

            var email = "user@email.com";
            var password = "Qu@dr@d0";
            var code = "code";

            var userResetPasswordByEmailApiDto = new UserResetPasswordByEmailApiDto();
            userResetPasswordByEmailApiDto.Email = email;
            userResetPasswordByEmailApiDto.Password = password;
            userResetPasswordByEmailApiDto.Code = code;

            var jsonString = JsonConvert.SerializeObject(userResetPasswordByEmailApiDto);
            jsonString = jsonString.Replace("\"", "\"\"");
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            var response = await Client.PutAsync("api/users/ResetPasswordByEmail", httpContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            #endregion
        }

        [Fact]
        public async Task CreatingUserWithEmailAndIncludingSameEmailWithUppercaselettersInTheMiddle_ShouldReturnOk()
        {
            #region Create User (Only Admin)

            // Arrange
            SetupAdminClaimsViaHeaders();

            var email = "user@email.com";
            var userDto = new UserDto<string>
            {
                UserName = email,
                Email = email
            };

            var jsonString = JsonConvert.SerializeObject(userDto);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            var createUserResponse = await Client.PostAsync("api/users", httpContent);

            // Assert
            createUserResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            #endregion

            #region Get User by Email

            // Arrange
            email = "useR@email.com";

            // Act
            createUserResponse = await Client.GetAsync($"api/Users/email/{email}");

            // Assert
            createUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            #endregion
        }

        [Fact]
        public async Task CreatingUserWithEmailAndIncludingSameEmailWithUppercaseletters_ShouldReturnOk()
        {
            #region Create User (Only Admin)

            // Arrange
            SetupAdminClaimsViaHeaders();

            var email = "user@email.com";
            var userDto = new UserDto<string>
            {
                UserName = email,
                Email = email
            };

            var jsonString = JsonConvert.SerializeObject(userDto);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            var createUserResponse = await Client.PostAsync("api/users", httpContent);

            // Assert
            createUserResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            #endregion

            #region Get User by Email

            // Arrange
            email = "USER@email.com";

            // Act
            createUserResponse = await Client.GetAsync($"api/Users/email/{email}");

            // Assert
            createUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            #endregion
        }

        [Fact]
        public async Task CreatingUserWithEmailAndIncludingSameEmailWithCaseInsensitiveComparison_ShouldReturnOk()
        {
            #region Create User (Only Admin)

            // Arrange
            SetupAdminClaimsViaHeaders();

            var email = "user@email.com";
            var userDto = new UserDto<string>
            {
                UserName = email,
                Email = email
            };

            var jsonString = JsonConvert.SerializeObject(userDto);
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            var createUserResponse = await Client.PostAsync("api/users", httpContent);

            // Assert
            createUserResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            #endregion

            #region Get User by Email

            // Arrange

            email = "UsEr@email.com";

            // Act
            createUserResponse = await Client.GetAsync($"api/Users/email/{email}");

            // Assert
            createUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            #endregion
        }
    }
}