using RecipeBook.Data.Models;
using RecipeBook.Data.Models.Builders;

namespace RecipeBookTest.Data;

public class RecipeBuilderTests
{
    [Fact]
    public void Build_ShouldReturnRecipeWithSetProperties()
    {
        // Arrange
        var category = new Category("Dessert");
        var user = new User { Id = "user1" };

        // Act
        var recipe = new RecipeBuilder()
            .SetName("Chocolate Cake")
            .SetTimeToCook(45)
            .SetInstructions("Mix and bake.")
            .SetImageUrl("http://image.jpg")
            .SetCategory(category)
            .SetUser(user)
            .Build();

        // Assert
        Assert.Equal("Chocolate Cake", recipe.Name);
        Assert.Equal(45, recipe.TimeToCook);
        Assert.Equal("Mix and bake.", recipe.Instructions);
        Assert.Equal("http://image.jpg", recipe.ImageUrl);
        Assert.Equal(category, recipe.Category);
        Assert.Equal(user, recipe.User);
    }

    [Fact]
    public void AddIngredient_ShouldAddIngredientToRecipe()
    {
        // Arrange
        var ingredient = new Ingredient("Flour", 2, "cups");

        // Act
        var recipe = new RecipeBuilder()
            .AddIngredient(ingredient)
            .Build();

        // Assert
        Assert.Contains(ingredient, recipe.Ingredients);
    }

    [Fact]
    public void AddLike_ShouldAddLikeToRecipe()
    {
        // Arrange
        var like = new Like(DateTime.UtcNow);
        like.User = new User { Id = "user1" };

        // Act
        var recipe = new RecipeBuilder()
            .AddLike(like)
            .Build();

        // Assert
        Assert.Contains(like, recipe.Likes);
    }

    [Fact]
    public void AddComment_ShouldAddCommentToRecipe()
    {
        // Arrange
        var comment = new Comment("Nice!", DateTime.UtcNow);

        // Act
        var recipe = new RecipeBuilder()
            .AddComment(comment)
            .Build();

        // Assert
        Assert.Contains(comment, recipe.Comments);
    }
}
