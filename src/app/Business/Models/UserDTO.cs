using System.ComponentModel.DataAnnotations;
using RecipeBook.Data.Models;

namespace RecipeBook.Business.Models;

public class UserDTO
{
    [Key]
    public string Id { get; set; }

    [Required]
    [MinLength(3)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Url]
    public string ImageUrl { get; set; } = string.Empty;

    public int RecipesCount { get; set; }

    public int CommentsCount { get; set; }

    public static UserDTO MapUser(User user)
    {
        return new UserDTO()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            ImageUrl = user.ImageUrl,
            RecipesCount = user.Recipes.Count,
            CommentsCount = user.Comments.Count
        };
    }
}
