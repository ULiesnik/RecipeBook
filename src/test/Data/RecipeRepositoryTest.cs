using Microsoft.EntityFrameworkCore;
using Moq;
using RecipeBook.Data.Context;
using RecipeBook.Data.Models;
using RecipeBook.Data.Repositories;

namespace RecipeBookTest.Data;

public class RecipeRepositoryTest
{
    private readonly Mock<DatabaseContext> _contextMock;
    private readonly Mock<DbSet<Recipe>> _recipeSetMock;
    private readonly RecipeRepository _repository;

    private readonly Recipe _testRecipe;
    private readonly User _testUser;
    private readonly Category _testCategory;

    public RecipeRepositoryTest()
    {
        _contextMock = new Mock<DatabaseContext>();
        _recipeSetMock = new Mock<DbSet<Recipe>>();
        _repository = new RecipeRepository(_contextMock.Object);

        _testUser = new User { Id = "user1", Recipes = new List<Recipe>() };
        _testCategory = new Category("test");
        _testCategory.Id = 1;
        _testRecipe = new Recipe { Id = 1, Name = "Pasta", User = _testUser, Category = _testCategory };

        var recipeList = new List<Recipe> { _testRecipe }.AsQueryable();
        _contextMock.Setup(c => c.Set<Recipe>()).Returns(_recipeSetMock.Object);
    }

    [Fact]
    public void Add_WithValidUserAndCategory_AddsRecipe()
    {
        _contextMock.Setup(c => c.Find<User>("user1")).Returns(_testUser);
        _contextMock.Setup(c => c.Find<Category>(1L)).Returns(_testCategory);
        _contextMock.Setup(c => c.SaveChanges()).Verifiable();

        var result = _repository.Add("user1", 1L, _testRecipe);

        Assert.NotNull(result);
        Assert.Contains(_testRecipe, _testUser.Recipes);
        Assert.Contains(_testRecipe, _testCategory.Recipes);
        _contextMock.Verify(c => c.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Add_WithInvalidUser_ReturnsNull()
    {
        _contextMock.Setup(c => c.Find<User>("invalid")).Returns((User?)null);

        var result = _repository.Add("invalid", 1L, _testRecipe);

        Assert.Null(result);
        _contextMock.Verify(c => c.SaveChanges(), Times.Never);
    }

    [Fact]
    public void Add_WithInvalidCategory_ReturnsNull()
    {
        _contextMock.Setup(c => c.Find<User>("user1")).Returns(_testUser);
        _contextMock.Setup(c => c.Find<Category>(999)).Returns((Category?)null);

        var result = _repository.Add("user1", 999, _testRecipe);

        Assert.Null(result);
        _contextMock.Verify(c => c.SaveChanges(), Times.Never);
    }

}