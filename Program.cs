using BookstoreApi;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//EF
builder.Services.AddDbContext<BookDb>(opt => opt.UseInMemoryDatabase("BookList"));

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});


app.MapPost("/books", async (Book b, BookDb db) =>
{
    if (b == null) return Results.BadRequest("Empty data");

    if (string.IsNullOrWhiteSpace(b.Title) || string.IsNullOrWhiteSpace(b.Author)) return Results.BadRequest("Author and Title must not be empty");

    if (decimal.IsNegative(b.Price)) return Results.BadRequest("Price is invalid");


    db.Books.Add(b);
    await db.SaveChangesAsync();

    return Results.Created($"/books/{b.Id}", b);
    
});

app.MapGet("/books/{id}", async (int id, BookDb db) =>
{
    var book = await db.Books.FindAsync(id);

    return book is not null ? Results.Ok(book) : Results.NotFound();

    
});

app.MapPut("/books/{id}", async (int id, Book book, BookDb db) =>
{
    var oldBook = await db.Books.FindAsync(id);

    if (oldBook is null) return Results.NotFound();

    oldBook.Author = book.Author;
    oldBook.Title = book.Title; 
    oldBook.Price = book.Price;
    oldBook.IsPublished = book.IsPublished;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/books/{id}", async (int id, BookDb db) =>
{
    var book = await db.Books.FindAsync(id);

    if (book is null) return Results.NotFound();

    db.Books.Remove(book);

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapGet("/books", async (BookDb db, string? author) =>
{
    if (author is null)
    {
        var allBooks = await db.Books.ToListAsync();
        return Results.Ok(allBooks);
    }

    var books = await db.Books.Where(b => b.Author.Equals(author)).ToListAsync();

    return Results.Ok(books);
});

app.Urls.Add("http://+:80");


app.Run();