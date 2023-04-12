﻿using RecipeBook.Data.Models;
using RecipeBook.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace RecipeBook.Business.Repositories;

public class RecipeRepository : EfCoreRepository<Recipe, DatabaseContext>
{
    private readonly DatabaseContext _context;

    public RecipeRepository(DatabaseContext context) : base(context)
    {
        _context = context;
    }

    public RecipeRepository() : this(new DatabaseContext())
    {
    }

    public new virtual List<Recipe>? GetAll()
    {
        return _context.Set<Recipe>().Include(r => r.Likes).
            Include(r => r.Category).Include(r => r.User).ToList();
    }

    public virtual List<Recipe>? GetUserRecipes(long id)
    {
        return _context.Set<Recipe>().Where(r => r.User!.Id == id).Include(r => r.Likes).
            Include(r => r.Category).Include(r => r.User).ToList();
    }

    public virtual List<Recipe>? GetLikedRecipes(long id)
    {
        var idList = _context.Set<Like>().Include(l => l.User).Include(l => l.Recipe).
            Where(l => l.User!.Id == id).Select(l => l.Recipe!.Id).ToList();
        return _context.Set<Recipe>().Where(r => idList.Contains(r.Id)).Include(r => r.Likes).
            Include(r => r.Category).Include(r => r.User).ToList();
    }

    public new virtual Recipe? Get(long id)
    {
        return _context.Set<Recipe>().Include(r => r.Likes).
            Include(r => r.Category).Include(r => r.User).Include(r => r.Ingredients).
            Include(r => r.Comments).FirstOrDefault(r => r.Id == id);
    }

    public virtual Recipe? Add(long userId, Recipe recipe)
    {
        var user = _context.Find<User>(userId);
        if (user == null)
        {
            return null;
        }
        user.Recipes.Add(recipe);
        _context.SaveChanges();
        return recipe;
    }
}
