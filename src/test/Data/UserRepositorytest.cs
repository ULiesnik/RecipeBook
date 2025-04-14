using Microsoft.EntityFrameworkCore;
using Moq;
using RecipeBook.Data.Context;
using RecipeBook.Data.Models;
using RecipeBook.Data.Repositories;

namespace RecipeBookTest.Data;

public class UserRepositoryTest
{
    private readonly Mock<DatabaseContext> _contextMock;
    private readonly Mock<DbSet<User>> _userSetMock;
    private readonly UserRepository _repository;

    private readonly User _testUser;

    public UserRepositoryTest()
    {
        _contextMock = new Mock<DatabaseContext>();
        _userSetMock = new Mock<DbSet<User>>();
        _repository = new UserRepository(_contextMock.Object);

        _testUser = new User
        {
            Id = "user1",
            UserName = "user1",
            Email = "test@example.com",
            Likes = new List<Like>(),
            Recipes = new List<Recipe>(),
            Comments = new List<Comment>()
        };

        var userData = new List<User> { _testUser }.AsQueryable();
        _userSetMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(userData.Provider);
        _userSetMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(userData.Expression);
        _userSetMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(userData.ElementType);
        _userSetMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(userData.GetEnumerator());

        _contextMock.Setup(c => c.Set<User>()).Returns(_userSetMock.Object);
    }

    [Fact]
    public void Get_ReturnsUserWithIncludes_WhenIdMatches()
    {
        _contextMock.Setup(c => c.Set<User>()).Returns(_userSetMock.Object);

        var result = _repository.Get("user1");

        Assert.NotNull(result);
        Assert.Equal("user1", result!.Id);
        Assert.NotNull(result.Likes);
        Assert.NotNull(result.Recipes);
        Assert.NotNull(result.Comments);
    }

    [Fact]
    public void Get_ReturnsNull_WhenUserNotFound()
    {
        var emptySet = new List<User>().AsQueryable();
        var emptyUserSetMock = new Mock<DbSet<User>>();
        emptyUserSetMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(emptySet.Provider);
        emptyUserSetMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(emptySet.Expression);
        emptyUserSetMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(emptySet.ElementType);
        emptyUserSetMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(emptySet.GetEnumerator());

        _contextMock.Setup(c => c.Set<User>()).Returns(emptyUserSetMock.Object);

        var result = _repository.Get("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ReturnsAllUsers()
    {
        var result = _repository.GetAll();

        Assert.NotNull(result);
        Assert.Single(result!);
        Assert.Equal("user1", result![0].Id);
    }

}
