using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

app.MapPost("/user", async (User user, AppDbContext context) =>
{
    if (string.IsNullOrWhiteSpace(user.Login))
    {
        return Results.BadRequest(new { message = "Логин не может быть пустым" });
    }

    if (string.IsNullOrWhiteSpace(user.PassHash))
    {
        return Results.BadRequest(new { message = "Хеш пароля не может быть пустым" });
    }

    bool loginExists = await context.Users.AnyAsync(u => u.Login == user.Login);

    if (loginExists)
    {
        return Results.BadRequest(new { message = "Пользователь с таким логином уже существует" });
    }

    context.Users.Add(user);
    await context.SaveChangesAsync();

    return Results.Created($"/user/{user.Id}", user);
});

app.MapGet("/user/{id}", async (int id, AppDbContext context) =>
{
    User? user = await context.Users.FindAsync(id);

    if (user == null)
    {
        return Results.NotFound(new { message = "Пользователь не найден" });
    }

    return Results.Ok(user);
});

app.MapPut("/user/{id}", async (int id, User updatedUser, AppDbContext context) =>
{
    User? user = await context.Users.FindAsync(id);

    if (user == null)
    {
        return Results.NotFound(new { message = "Пользователь не найден" });
    }

    if (string.IsNullOrWhiteSpace(updatedUser.Login))
    {
        return Results.BadRequest(new { message = "Логин не может быть пустым" });
    }

    if (string.IsNullOrWhiteSpace(updatedUser.PassHash))
    {
        return Results.BadRequest(new { message = "Хеш пароля не может быть пустым" });
    }

    bool loginExists = await context.Users
        .AnyAsync(u => u.Login == updatedUser.Login && u.Id != id);

    if (loginExists)
    {
        return Results.BadRequest(new { message = "Пользователь с таким логином уже существует" });
    }

    user.Login = updatedUser.Login;
    user.PassHash = updatedUser.PassHash;

    await context.SaveChangesAsync();

    return Results.Ok(user);
});

app.MapDelete("/user/{id}", async (int id, AppDbContext context) =>
{
    User? user = await context.Users.FindAsync(id);

    if (user == null)
    {
        return Results.NotFound(new { message = "Пользователь не найден" });
    }

    context.Users.Remove(user);
    await context.SaveChangesAsync();

    return Results.Ok(new { message = "Пользователь удален" });
});

app.Run();

class User
{
    public int Id { get; set; }
    public string Login { get; set; } = "";
    public string PassHash { get; set; } = "";
}

class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=users.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Login)
            .IsUnique();
    }
}
