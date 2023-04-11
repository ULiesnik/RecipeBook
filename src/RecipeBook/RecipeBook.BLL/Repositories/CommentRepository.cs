﻿using Microsoft.EntityFrameworkCore;
using RecipeBook.DAL.Data;
using RecipeBook.DAL.Models;

namespace RecipeBook.BLL.Repositories;
public class CommentRepository : EfCoreRepository<Comment, DatabaseContext>
{
    private readonly DatabaseContext _context;
    public CommentRepository(DatabaseContext context) : base(context)
    {
        _context = context;
    }
    public Comment Add(long userId, long recipeId, string text)
    {
        Recipe? recipe = _context.Find<Recipe>(recipeId);
        if (recipe != null)
        {
            User? user = _context.Find<User>(userId);
            if (user != null)
            {
                Comment comment = new Comment(text, DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc));
                user.Comments.Add(comment);
                recipe.Comments.Add(comment);
                _context.SaveChanges();
                return comment;
            }
        }
        return null;
    }
}