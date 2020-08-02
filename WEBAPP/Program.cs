using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ConsoleApp2
{
    public class Library
    {
        public int Id { get; set; }
        public string Address { get; set; }

        public List<Book> Books { get; } = new List<Book>();
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }

        public int LibraryId { get; set; }
        public Library Library { get; set; }
    }

    class Program
    {
        static HttpClient client = new HttpClient();

        // Call API to add library to db
        static async Task<Uri> CreateLibraryAsync(Library library)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/Library", library);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        // Call API to add book to db
        static async Task<Uri> CreateBookAsync(Book book)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/Book", book);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        // Call API to get book
        static async Task<Book> GetBookAsync(string path)
        {
            Book book = null;
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                book = await response.Content.ReadAsAsync<Book>();
            }

            return book;
        }

        // Call API to get a Library 
        static async Task<Library> GetLibraryAsync(string path)
        {
            Library library = null;
            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                library = await response.Content.ReadAsAsync<Library>();
            }

            return library;
        }

        // Get all books
        static async Task<IEnumerable<Book>> GetAllBookAsync()
        {
            List<Book> books = null;
            HttpResponseMessage response = await client.GetAsync("http://127.0.0.1:1784/api/Book");

            if (response.IsSuccessStatusCode)
            {
                books = await response.Content.ReadAsAsync<List<Book>>();
            }

            return books;
        }

        // Get all libraries
        static async Task<IEnumerable<Library>> GetAllLibraryAsync()
        {
            List<Library> libraries = null;
            HttpResponseMessage response = await client.GetAsync("http://127.0.0.1:1784/api/Library");

            if (response.IsSuccessStatusCode)
            {
                libraries = await response.Content.ReadAsAsync<List<Library>>();
            }

            return libraries;
        }

        // Call API to update Book
        static async Task<Book> UpdateBookAsync(Book book)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                 $"api/Book/{book.Id}", book);

            book = await response.Content.ReadAsAsync<Book>();

            return book;
        }

        // Call API to update Library
        static async Task<Library> UpdateLibraryAsync(Library library)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                 $"api/Library/{library.Id}", library);
            response.EnsureSuccessStatusCode();

            library = await response.Content.ReadAsAsync<Library>();

            return library;
        }

        // Call API to delete book
        static async Task<HttpStatusCode> DeleteBookAsync(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/Book/{id}");
            return response.StatusCode;
        }

        // Call API to delete Library
        static async Task<HttpStatusCode> DeleteLibraryAsync(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/Library/{id}");
            return response.StatusCode;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Assignment #2 - API Requests");
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://127.0.0.1:1784/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Create a library
                Library library = new Library
                {
                    Id = 1,
                    Address = "123 Sunny Street"
                };

                Library library2 = new Library
                {
                   Id = 2,
                    Address = "789 John Rd."
                };

                // Create a book for that library
                Book book = new Book
                {
                    Id = 1,
                    Title = "123 Counting",
                    Summary = "Count with me",
                    LibraryId = 1
                };

                Book book2 = new Book
                {
                    Id = 2,
                    Title = "678 Tree",
                    Summary = "The forest",
                    LibraryId = 2
                };

                // Add library to db
                var urlLib = await CreateLibraryAsync(library);
                Console.WriteLine($"Added a library at {urlLib}");

                // Add book
                var urlBook = await CreateBookAsync(book);
                Console.WriteLine($"Added a book at {urlBook}");

                // Get Library
                library = await GetLibraryAsync(urlLib.PathAndQuery);
                Console.WriteLine("Library");
                Console.WriteLine(library.Id);
                Console.WriteLine(library.Address);

                // Get Book
                book = await GetBookAsync(urlBook.PathAndQuery);
                Console.WriteLine("Book");
                Console.WriteLine(book.Id);
                Console.WriteLine(book.Title);
                Console.WriteLine(book.Summary);
                Console.WriteLine(book.LibraryId);

                // Get all books & print them out
                IEnumerable<Book> books = await GetAllBookAsync();

                Console.WriteLine("Books -------- ");
                foreach (var b in books)
                {
                    Console.WriteLine(b.Title);
                }

                // Get all libraries and print them out
                IEnumerable<Library> libraries = await GetAllLibraryAsync();

                Console.WriteLine("Libraries -------- ");
                foreach (var l in libraries)
                {
                    Console.WriteLine(l.Address);
                }

                // Update Library
                library.Address = "New Address woohoo";
                await UpdateLibraryAsync(library);

                Console.WriteLine("Library");
                Console.WriteLine(library.Id);
                Console.WriteLine(library.Address);

                // Update Book
                book.Title = "123 45678910!!!";
                await UpdateBookAsync(book);

                Console.WriteLine("Book");
                Console.WriteLine(book.Id);
                Console.WriteLine(book.Title);
                Console.WriteLine(book.Summary);
                Console.WriteLine(book.LibraryId);


                // Delete Library
                var statusCodeLib = await DeleteLibraryAsync(library.Id);
                Console.WriteLine($"Deleted - Status Code = {(int)statusCodeLib}");

                // Delete Book
                var statusCodeBook = await DeleteBookAsync(book.Id);
                Console.WriteLine($"Deleted - Status Code = {(int)statusCodeBook}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }


}
