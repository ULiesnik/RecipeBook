namespace RecipeBook.Data.Repositories;

using Context;
using Models;

public class IngredientRepository : EfCoreRepository<Ingredient, DatabaseContext>
{
    private readonly DatabaseContext _context;

    public IngredientRepository(DatabaseContext context) : base(context)
    {
        _context = context;
    }

    public virtual Ingredient? Add(long recipeId, Ingredient ingredient)
    {
        var recipe = _context.Find<Recipe>(recipeId);
        if (recipe == null)
        {
            return null;
        }

        recipe.Ingredients.Add(ingredient);
        _context.SaveChanges();
        return ingredient;
    }
}