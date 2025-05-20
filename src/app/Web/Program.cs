using Microsoft.EntityFrameworkCore;
using RecipeBook.Business.Services;
using RecipeBook.Business.Models;
using RecipeBook.Data.Context;
using RecipeBook.Data.Repositories;
using RecipeBook.Data.Models;
using Serilog;
using Microsoft.AspNetCore.Identity;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration().WriteTo.Console()
        .WriteTo.Seq(builder.Configuration["Logging:Serilog:Seq"]).CreateLogger();
    Log.Information("Starting");

    builder.Host.UseSerilog();

    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseNpgsql(connectionString));

    builder.Services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();

    var dbContext = builder.Services.BuildServiceProvider().GetService<DatabaseContext>();

    var recipeRepository = new RecipeRepository(dbContext!);
    var userRepository = new UserRepository(dbContext!);
    var ingredientRepository = new IngredientRepository(dbContext!);
    var commentRepository = new CommentRepository(dbContext!);
    var likeRepository = new LikeRepository(dbContext!);
    var categoryRepository = new CategoryRepository(dbContext!);

    builder.Services.AddSingleton<IRecipeService>(_ =>
        new RecipeService(recipeRepository));
    builder.Services.AddSingleton<IUserService>(_ =>
        new UserService(userRepository));
    builder.Services.AddSingleton<IIngredientService>(_ =>
        new IngredientService(ingredientRepository));
    builder.Services.AddSingleton<ICommentService>(_ =>
        new CommentService(commentRepository));
    builder.Services.AddSingleton<ILikeService>(_ =>
        new LikeService(likeRepository, recipeRepository, userRepository));
    builder.Services.AddSingleton<ICategoryService>(_ =>
        new CategoryService(categoryRepository));

    builder.Services.AddScoped<IAccountService, AccountService>();
    builder.Services.AddScoped<IContextService, ContextService>();
    builder.Services.AddScoped<IEmailService, EmailService>();

    builder.Services.ConfigureApplicationCookie(config =>
        config.LoginPath = builder.Configuration["Application:LoginPath"]);

    builder.Services.Configure<SMTPConfigModel>(builder.Configuration.GetSection("SMTPConfig"));

    builder.Services.Configure<IdentityOptions>(options =>
        options.SignIn.RequireConfirmedEmail = true);

    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "App terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
