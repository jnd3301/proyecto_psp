using api;
using Microsoft.EntityFrameworkCore;
using test;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EmailDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/todoitems", async (EmailDb db) =>
    await db.Todos.ToListAsync());

//app.MapGet("/todoitems/complete", async (EmailDb db) =>
//    await db.Todos.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, EmailDb db) =>
    await db.Todos.FindAsync(id)
        is Email todo
            ? Results.Ok(todo)
            : Results.NotFound());

app.MapPost("/todoitems", async (Email todo, EmailDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (int id, Email inputTodo, EmailDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Cuerpo = inputTodo.Cuerpo;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, EmailDb db) =>
{
    if (await db.Todos.FindAsync(id) is Email todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.Ok(todo);
    }

    return Results.NotFound();
});

app.Run(app.Configuration["Data:Url"]);
