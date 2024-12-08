using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AuthService.BLL.Services;
using AuthService.Domain.Enities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace AuthService.Tests.Services
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(config => config["Jwt:Secret"]).Returns("SuperSecretKey12345!");
            _mockConfiguration.Setup(config => config["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfiguration.Setup(config => config["Jwt:Audience"]).Returns("TestAudience");
            _mockConfiguration.Setup(config => config["Jwt:Expire"]).Returns("1");

            _tokenService = new TokenService(_mockConfiguration.Object);
        }
        
        [Fact]
        public void TranslateRoleToEnglish_ShouldTranslateKnownRoles()
        {
            // Arrange
            var privateMethod = typeof(TokenService)
                .GetMethod("TranslateRoleToEnglish", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act & Assert
            Assert.Equal("Department Head", privateMethod.Invoke(_tokenService, new object[] { "Начальник подразделения" }));
            Assert.Equal("Resident", privateMethod.Invoke(_tokenService, new object[] { "Пользователь" }));
            Assert.Equal("Warehouse Manager", privateMethod.Invoke(_tokenService, new object[] { "Начальник склада" }));
            Assert.Equal("Accountant", privateMethod.Invoke(_tokenService, new object[] { "Бухгалтер" }));
        }

        [Fact]
        public void TranslateRoleToEnglish_ShouldReturnOriginalRoleIfUnknown()
        {
            // Arrange
            var privateMethod = typeof(TokenService)
                .GetMethod("TranslateRoleToEnglish", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = privateMethod.Invoke(_tokenService, new object[] { "Unknown Role" });

            // Assert
            Assert.Equal("Unknown Role", result);
        }
        
        [Fact]
        public void GenerateAccessToken_ShouldThrowIfInvalidConfiguration()
        {
            // Arrange
            var invalidConfig = new Mock<IConfiguration>();
            invalidConfig.Setup(config => config["Jwt:Secret"]).Returns((string)null);

            var invalidService = new TokenService(invalidConfig.Object);
            var user = new User { Id = Guid.NewGuid(), Login = "testuser" };
            var roles = new List<Role> { new Role { Name = "Начальник подразделения" } };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => invalidService.GenerateAccessToken(user, roles));
        }
    }
}
