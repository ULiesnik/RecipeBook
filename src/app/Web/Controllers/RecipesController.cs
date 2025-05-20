namespace RecipeBook.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Business.Services;
using Business.Models;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;

public class RecipesController : Controller
{
    private readonly IRecipeService _recipeService;
    private readonly ILikeService _likeService;
    private readonly ICommentService _commentService;
    private readonly ICategoryService _categoryService;
    private readonly IIngredientService _ingredientService;
    private readonly IContextService _contextService;

    public RecipesController(IRecipeService recipeService, ICommentService commentService,
        ILikeService likeService, ICategoryService categoryService,
        IIngredientService ingredientService, IContextService contextService)
    {
        _recipeService = recipeService;
        _commentService = commentService;
        _likeService = likeService;
        _categoryService = categoryService;
        _ingredientService = ingredientService;
        _contextService = contextService;
    }

    [HttpGet]
    public IActionResult AllRecipes(string sortingField = "")
    {
        var list = _recipeService.GetAllRecipes();
        return View("Recipes", _recipeService.GetRecipesSortedBy(sortingField, list!));
    }

    [Authorize]
    [HttpGet("UserRecipes/{id}")]
    public IActionResult UserRecipes(string id, string sortingField = "")
    {
        var list = _recipeService.GetUserRecipes(id);
        return View("UserRecipes", _recipeService.GetRecipesSortedBy(sortingField, list!));
    }

    [Authorize]
    [HttpGet]
    public IActionResult MyRecipes(string sortingField = "")
    {
        string _currentId = _contextService.GetUserId();
        var list = _recipeService.GetUserRecipes(_currentId);
        return View("MyRecipes", _recipeService.GetRecipesSortedBy(sortingField, list!));
    }

    [Authorize]
    [HttpGet]
    public IActionResult LikedRecipes(string sortingField = "")
    {
        string _currentId = _contextService.GetUserId();
        var list = _recipeService.GetLikedRecipes(_currentId);
        return View("LikedRecipes", _recipeService.GetRecipesSortedBy(sortingField, list!));
    }

    [Authorize]
    [HttpGet("Recipe/{id:long}")]
    public IActionResult Recipe(long id)
    {
        ViewBag.CurrentUserId = _contextService.GetUserId();
        return View("Recipe", _recipeService.GetRecipe(id));
    }

    [Authorize]
    public IActionResult DeleteRecipe(long recipeId)
    {
        _recipeService.DeleteRecipe(recipeId);
        string _currentId = _contextService.GetUserId();
        var list = _recipeService.GetUserRecipes(_currentId);
        ViewBag.RecipeDeleted = true;
        return View("MyRecipes", list);
    }

    [Authorize]
    public IActionResult DeleteComment(long commentId, long recipeId)
    {
        _commentService.DeleteComment(commentId);
        ViewBag.CurrentUserId = _contextService.GetUserId();
        return View("Recipe", _recipeService.GetRecipe(recipeId));
    }

    [Authorize]
    [HttpGet]
    public IActionResult CreateRecipe()
    {
        ViewBag.Categories = _categoryService.GetAll();
        return View("CreateRecipe");
    }

    [Authorize]
    [HttpPost]
    public IActionResult AddRecipe(RecipeDetailsDTO recipe)
    {
        string _currentId = _contextService.GetUserId();
        var newRecipe = _recipeService.AddRecipe(_currentId, recipe.CategoryId, recipe);
        return View("AddRecipeIngredients", newRecipe);
    }

    [Authorize]
    [HttpPost]
    public IActionResult AddNewCategory(string name)
    {
        CategoryDTO category = _categoryService.AddNewCategory(name);
        ViewBag.CategoryAdded = true;
        ViewBag.Categories = _categoryService.GetAll();
        return View("CreateRecipe");
    }

    [Authorize]
    [HttpGet]
    [HttpPost]
    public IActionResult AddIngredient(long recipeId, string name, int quantity, string measure = "")
    {
        _ingredientService.AddIngredient(recipeId, name, quantity, measure);
        RecipeDetailsDTO recipe = _recipeService.GetRecipe(recipeId);
        return View("AddRecipeIngredients", recipe);
    }

    [Authorize]
    [HttpPost]
    public IActionResult AddComment(long recipeId, string comment)
    {
        string _currentId = _contextService.GetUserId();
        _commentService.AddComment(_currentId, recipeId, comment);
        var recipe = _recipeService.GetRecipe(recipeId);
        ViewBag.CurrentUserId = _contextService.GetUserId();
        return View("Recipe", recipe);
    }

    [Authorize]
    [HttpPost]
    public IActionResult AddOrDeleteLike(long recipeId)
    {
        string _currentId = _contextService.GetUserId();
        if (!_likeService.AddLike(_currentId, recipeId))
        {
            if (_likeService.DeleteLike(_currentId, recipeId))
            {
                ViewBag.LikeDeleted = true;
            }
        }
        else
        {
            ViewBag.LikeAdded = true;
        }
        var recipe = _recipeService.GetRecipe(recipeId);
        ViewBag.CurrentUserId = _currentId;
        return View("Recipe", recipe);
    }
}
