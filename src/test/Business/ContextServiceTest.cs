using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RecipeBook.Business.Services;

namespace RecipeBookTest.Business
{
    public class ContextServiceTests
    {
        [Fact]
        public void GetUserId_ReturnsCorrectUserId_WhenUserIsAuthenticated()
        {
            // Arrange
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            var claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "12345")
            });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            mockHttpContext.Setup(ctx => ctx.User).Returns(claimsPrincipal);
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

            var contextService = new ContextService(mockHttpContextAccessor.Object);

            // Act
            var userId = contextService.GetUserId();

            // Assert
            Assert.Equal("12345", userId);
        }

        [Fact]
        public void GetUserId_ReturnsNull_WhenUserIsNotAuthenticated()
        {
            // Arrange
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var mockHttpContext = new Mock<HttpContext>();
            var claimsPrincipal = new ClaimsPrincipal(); // Empty claims
            mockHttpContext.Setup(ctx => ctx.User).Returns(claimsPrincipal);
            mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);

            var contextService = new ContextService(mockHttpContextAccessor.Object);

            // Act
            var userId = contextService.GetUserId();

            // Assert
            Assert.Null(userId);
        }
    }
}
