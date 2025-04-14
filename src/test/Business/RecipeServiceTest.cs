using RecipeBook.Business.Models;
using RecipeBook.Data.Models;
using RecipeBookTest.Business.Proxy;

namespace RecipeBookTest.Business;

using Moq;
using RecipeBook.Business.Services;

public sealed class RecipeServiceTest
{
    readonly Mock<RecipeRepositoryMoqProxy> _recipeRepositoryMock;
    readonly RecipeService _recipeService;
    readonly string _wrongUid;
    readonly long _wrongRecipeId;

    readonly Recipe _testRecipe1 = new()
    {
        Name = "Recipe 1",
        Instructions = "Instructions1",
        TimeToCook = 10,
        ImageUrl = "Test1",
        Category = new Category("Test1"),
    };

    private readonly RecipeDetailsDTO _testRecipeDetailsDTO1 = new()
    {
        Name = "Recipe 1",
        Instructions = "Instructions1",
        TimeToCook = 10,
        ImageUrl = "Test1",
        Category = new Category("Test1"),
    };

    readonly Recipe _testRecipe2 = new()
    {
        Name = "Recipe 2",
        Instructions = "Instructions2",
        TimeToCook = 20,
        ImageUrl = "Test2",
        Category = new Category("Test2"),
    };

    readonly Recipe _testRecipe3 = new()
    {
        Name = "Recipe 3",
        Instructions = "Instructions3",
        TimeToCook = 30,
        ImageUrl = "Test3",
        Category = new Category("Test3"),
    };

    public RecipeServiceTest()
    {
        _recipeRepositoryMock = new Mock<RecipeRepositoryMoqProxy>();
        InitMockMethods();
        _recipeService = new(_recipeRepositoryMock.Object);
        _wrongUid = "-1";
        _wrongRecipeId = -1L;
    }

    void InitMockMethods()
    {
        _recipeRepositoryMock.Setup(r =>
            r.GetAll()).Returns(
            new List<Recipe>
            {
                _testRecipe1,
                _testRecipe2,
                _testRecipe3,
            }
        );

        _recipeRepositoryMock.Setup(r =>
            r.GetUserRecipes(It.IsAny<string>())).Returns(
            (string uid) => uid == _wrongUid
                ? null
                : new List<Recipe>
                {
                    _testRecipe1,
                    _testRecipe3,
                }
        );

        _recipeRepositoryMock.Setup(r =>
            r.GetUserLikedRecipes(It.IsAny<string>())).Returns(
            new List<Recipe>
            {
                _testRecipe1,
            }
        );

        _recipeRepositoryMock.Setup(r =>
            r.Get(It.IsAny<long>())).Returns(
            (long id) => id == _wrongRecipeId
                ? null
                : _testRecipe1
        );

        _recipeRepositoryMock.Setup(r =>
            r.Add(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<Recipe>())).Returns(
            (string uid, long recipeId, Recipe recipe) =>
                uid == _wrongUid || recipeId == _wrongRecipeId
                    ? null
                    : recipe
        );
    }

    [Fact]
    public void GetAllRecipes_ReturnsListOf3()
    {
        var recipes = _recipeService.GetAllRecipes();
        Assert.Equal(3, recipes?.Count);
    }

    [Fact]
    public void GetUserRecipes_ReturnsListOf2()
    {
        var recipes = _recipeService.GetUserRecipes("1");
        Assert.Equal(2, recipes?.Count);
    }

    [Fact]
    public void GetUserRecipes_ReturnsNull()
    {
        var recipes = _recipeService.GetUserRecipes(_wrongUid);
        Assert.Null(recipes);
    }

    [Fact]
    public void GetLikedRecipes_ReturnsListOf1()
    {
        var recipes = _recipeService.GetLikedRecipes("1");
        Assert.Equal(1, recipes?.Count);
    }

    [Fact]
    public void GetRecipesSortedByLikes_ReturnsList()
    {
        var recipes = _recipeService.GetRecipesSortedBy("Likes", _recipeService.GetAllRecipes()!);
        Assert.Equal("Recipe 1", recipes?[0].Name);
        Assert.Equal("Recipe 2", recipes?[1].Name);
        Assert.Equal("Recipe 3", recipes?[2].Name);
    }

    [Fact]
    public void GetRecipesSortedByLComments_ReturnsList()
    {
        var recipes =
            _recipeService.GetRecipesSortedBy("Comments", _recipeService.GetAllRecipes()!);
        Assert.Equal("Recipe 1", recipes?[0].Name);
        Assert.Equal("Recipe 2", recipes?[1].Name);
        Assert.Equal("Recipe 3", recipes?[2].Name);
    }

    [Fact]
    public void GetRecipesSortedByTimeToCook_ReturnsList()
    {
        var recipes =
            _recipeService.GetRecipesSortedBy("TimeToCook", _recipeService.GetAllRecipes()!);
        Assert.Equal("Recipe 1", recipes?[0].Name);
        Assert.Equal("Recipe 2", recipes?[1].Name);
        Assert.Equal("Recipe 3", recipes?[2].Name);
    }
    

    [Fact]
    public void GetRecipe_ReturnsRecipe1()
    {
        var recipe = _recipeService.GetRecipe(1);
        Assert.Equal(_testRecipe1.Name, recipe?.Name);
        Assert.Equal(_testRecipe1.Instructions, recipe?.Instructions);
        Assert.Equal(_testRecipe1.TimeToCook, recipe?.TimeToCook);
    }

    [Fact]
    public void GetRecipe_ReturnsNull()
    {
        var recipe = _recipeService.GetRecipe(_wrongRecipeId);
        Assert.Null(recipe);
    }

    [Fact]
    public void AddRecipe_ReturnsTestRecipeDetailsDTO1()
    {
        var recipe = _recipeService.AddRecipe("1", 1L,
            _testRecipeDetailsDTO1);
        Assert.Equal(_testRecipeDetailsDTO1.Name, recipe?.Name);
        Assert.Equal(_testRecipeDetailsDTO1.Instructions, recipe?.Instructions);
        Assert.Equal(_testRecipeDetailsDTO1.TimeToCook, recipe?.TimeToCook);
    }

    [Fact]
    public void AddRecipe_WithWrongUid_ReturnsNull()
    {
        var recipe = _recipeService.AddRecipe(_wrongUid, _wrongRecipeId,
            _testRecipeDetailsDTO1);
        Assert.Null(recipe);
    }
}