using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

// Сущность Автор
class Author
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public List<Book> Books { get; set; } = new List<Book>();
}

// Сущность Книга
class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public decimal Price { get; set; }

    public int AuthorId { get; set; }
    public Author? Author { get; set; }
}

// Контекст базы данных
class AppDbContext : DbContext
{
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=library.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>()
            .HasMany(a => a.Books)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId);
    }
}

// Обобщенный интерфейс репозитория
interface IRepository<T> where T : class
{
    T? GetById(int id);
    IEnumerable<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);

    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task AddAsync(T entity);
}

// Специализированный интерфейс для книг
interface IBookRepository : IRepository<Book>
{
    IEnumerable<Book> GetBooksByAuthor(string authorName);
    IEnumerable<Book> GetBooksByPriceRange(decimal minPrice, decimal maxPrice);
}

// Универсальный репозиторий
class GenericRepository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public T? GetById(int id)
    {
        return _dbSet.Find(id);
    }

    public IEnumerable<T> GetAll()
    {
        return _dbSet.ToList();
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
        _context.SaveChanges();
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        T? entity = _dbSet.Find(id);

        if (entity != null)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}

// Репозиторий для книг
class BookRepository : GenericRepository<Book>, IBookRepository
{
    public BookRepository(AppDbContext context) : base(context)
    {
    }

    public new Book? GetById(int id)
    {
        return _context.Books
            .Include(b => b.Author)
            .FirstOrDefault(b => b.Id == id);
    }

    public new IEnumerable<Book> GetAll()
    {
        return _context.Books
            .Include(b => b.Author)
            .ToList();
    }

    public IEnumerable<Book> GetBooksByAuthor(string authorName)
    {
        return _context.Books
            .Include(b => b.Author)
            .Where(b => b.Author != null && b.Author.Name.Contains(authorName))
            .ToList();
    }

    public IEnumerable<Book> GetBooksByPriceRange(decimal minPrice, decimal maxPrice)
    {
        return _context.Books
            .Include(b => b.Author)
            .Where(b => b.Price >= minPrice && b.Price <= maxPrice)
            .ToList();
    }
}

// Unit of Work
class UnitOfWork : IDisposable
{
    private readonly AppDbContext _context;

    public IBookRepository Books { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Books = new BookRepository(_context);
    }

    public void Save()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Лабораторная работа №2. Часть 2. Репозиторий");
        Console.WriteLine();

        using AppDbContext context = new AppDbContext();

        // Для демонстрации база каждый раз создается заново
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        Console.WriteLine("База данных SQLite создана.");
        Console.WriteLine();

        using UnitOfWork unitOfWork = new UnitOfWork(context);

        Author author1 = new Author { Name = "Лев Толстой" };
        Author author2 = new Author { Name = "Федор Достоевский" };

        context.Authors.Add(author1);
        context.Authors.Add(author2);
        context.SaveChanges();

        Book book1 = new Book
        {
            Title = "Война и мир",
            Price = 1200,
            AuthorId = author1.Id
        };

        Book book2 = new Book
        {
            Title = "Анна Каренина",
            Price = 900,
            AuthorId = author1.Id
        };

        Book book3 = new Book
        {
            Title = "Преступление и наказание",
            Price = 800,
            AuthorId = author2.Id
        };

        Console.WriteLine("Добавление книг в базу данных:");
        unitOfWork.Books.Add(book1);
        unitOfWork.Books.Add(book2);
        await unitOfWork.Books.AddAsync(book3);

        PrintBooks(unitOfWork.Books.GetAll());

        Console.WriteLine();
        Console.WriteLine("Получение книги по Id:");
        Book? foundBook = unitOfWork.Books.GetById(1);

        if (foundBook != null)
        {
            Console.WriteLine($"Найдена книга: {foundBook.Title}, автор: {foundBook.Author?.Name}, цена: {foundBook.Price}");
        }

        Console.WriteLine();
        Console.WriteLine("Обновление книги:");
        if (foundBook != null)
        {
            foundBook.Price = 1300;
            unitOfWork.Books.Update(foundBook);
        }

        PrintBooks(unitOfWork.Books.GetAll());

        Console.WriteLine();
        Console.WriteLine("Поиск книг по автору 'Толстой':");
        IEnumerable<Book> tolstoyBooks = unitOfWork.Books.GetBooksByAuthor("Толстой");
        PrintBooks(tolstoyBooks);

        Console.WriteLine();
        Console.WriteLine("Поиск книг по диапазону цен от 700 до 1000:");
        IEnumerable<Book> booksByPrice = unitOfWork.Books.GetBooksByPriceRange(700, 1000);
        PrintBooks(booksByPrice);

        Console.WriteLine();
        Console.WriteLine("Удаление книги с Id = 2:");
        unitOfWork.Books.Delete(2);

        PrintBooks(unitOfWork.Books.GetAll());

        Console.WriteLine();
        Console.WriteLine("Асинхронное получение списка книг:");
        List<Book> asyncBooks = await unitOfWork.Books.GetAllAsync();
        PrintBooks(asyncBooks);

        Console.WriteLine();
        Console.WriteLine("Программа завершена.");
    }

    static void PrintBooks(IEnumerable<Book> books)
    {
        foreach (Book book in books)
        {
            Console.WriteLine($"Id: {book.Id}; Название: {book.Title}; Автор: {book.Author?.Name}; Цена: {book.Price} руб.");
        }
    }
}
