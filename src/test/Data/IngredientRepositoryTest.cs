using Microsoft.EntityFrameworkCore;
using Moq;
using RecipeBook.Data.Context;
using RecipeBook.Data.Models;
using RecipeBook.Data.Repositories;

namespace RecipeBookTest.Data;

public class IngredientRepositoryTest
{
    private readonly Mock<DatabaseContext> _contextMock;
    private readonly Mock<DbSet<Ingredient>> _ingredientSetMock;
    private readonly IngredientRepository _repository;

    private readonly Recipe _testRecipe;
    private readonly Ingredient _testIngredient;

    public IngredientRepositoryTest()
    {
        _contextMock = new Mock<DatabaseContext>();
        _ingredientSetMock = new Mock<DbSet<Ingredient>>();
        _repository = new IngredientRepository(_contextMock.Object);

        _testRecipe = new Recipe { Id = 1, Ingredients = new List<Ingredient>() };
        _testIngredient = new Ingredient("Tomato", 2, "pcs");
        _testIngredient.Id = 1;

        var ingredientList = new List<Ingredient> { _testIngredient }.AsQueryable();

        _ingredientSetMock.As<IQueryable<Ingredient>>().Setup(m => m.Provider).Returns(ingredientList.Provider);
        _ingredientSetMock.As<IQueryable<Ingredient>>().Setup(m => m.Expression).Returns(ingredientList.Expression);
        _ingredientSetMock.As<IQueryable<Ingredient>>().Setup(m => m.ElementType).Returns(ingredientList.ElementType);
        _ingredientSetMock.As<IQueryable<Ingredient>>().Setup(m => m.GetEnumerator()).Returns(ingredientList.GetEnumerator());

        _contextMock.Setup(c => c.Set<Ingredient>()).Returns(_ingredientSetMock.Object);
    }

    [Fact]
    public void Add_WithValidRecipeId_AddsIngredientToRecipe()
    {
        _contextMock.Setup(c => c.Find<Recipe>(1L)).Returns(_testRecipe);
        _contextMock.Setup(c => c.SaveChanges()).Verifiable();

        var result = _repository.Add(1L, _testIngredient);

        Assert.NotNull(result);
        Assert.Contains(_testIngredient, _testRecipe.Ingredients);
        _contextMock.Verify(c => c.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Add_WithInvalidRecipeId_ReturnsNull()
    {
        _contextMock.Setup(c => c.Find<Recipe>(999L)).Returns((Recipe?)null);

        var result = _repository.Add(999L, _testIngredient);

        Assert.Null(result);
        _contextMock.Verify(c => c.SaveChanges(), Times.Never);
    }


    [Fact]
    public void Get_ReturnsCorrectIngredient()
    {
        _ingredientSetMock.Setup(m => m.Find(1L)).Returns(_testIngredient);

        var result = _repository.Get(1L);

        Assert.NotNull(result);
        Assert.Equal("Tomato", result!.Name);
    }

    [Fact]
    public void GetAll_ReturnsAllIngredients()
    {
        var result = _repository.GetAll();

        Assert.Single(result!);
        Assert.Equal("Tomato", result![0].Name);
    }


    [Fact]
    public void Delete_RemovesIngredient()
    {
        _ingredientSetMock.Setup(m => m.Find(1L)).Returns(_testIngredient);
        _ingredientSetMock.Setup(m => m.Remove(_testIngredient)).Verifiable();
        _contextMock.Setup(m => m.SaveChanges()).Verifiable();

        var result = _repository.Delete(1L);

        _ingredientSetMock.Verify(m => m.Remove(_testIngredient), Times.Once);
        _contextMock.Verify(m => m.SaveChanges(), Times.Once);
        Assert.Equal("Tomato", result!.Name);
    }
}
