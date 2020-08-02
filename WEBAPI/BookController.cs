using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookDbContext _context;

        public BookController(BookDbContext context)
        {
            _context = context;
        }

        // Returns all books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks()
        {
            return await _context.Books
                .Select(x => BookDTO(x))
                .ToListAsync();
        }

        // Returns book based on incoming ID
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBook(long id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return BookDTO(book);
        }

        // Updates book based on incoming ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(long id, BookDTO bookDTO)
        {
            if (id != bookDTO.Id)
            {
                return BadRequest();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            book.Id = bookDTO.Id;
            book.Title = bookDTO.Title;
            book.Summary = bookDTO.Summary;
            book.LibraryId = bookDTO.LibraryId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!BookExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // Creates a new book and adds to db
        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook(BookDTO bookDTO)
        {
            var book = new Book
            {
                Title = bookDTO.Title,
                Summary = bookDTO.Summary,
                LibraryId = bookDTO.LibraryId,
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetBook),
                new { id = book.Id },
                BookDTO(book));
        }

        // Removes book from db based on incoming id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(long id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Creating an instance of BookDTO from incoming book model
        private static BookDTO BookDTO(Book book) =>
            new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Summary = book.Summary,
                LibraryId = book.LibraryId
            };

        // Check if book
        private bool BookExists(long id) =>
            _context.Books.Any(e => e.Id == id);
    }
}

