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
    public class LibraryController : ControllerBase
    {
        private readonly LibraryDbContext _context;

        public LibraryController(LibraryDbContext context)
        {
            _context = context;
        }

        // Returns all libraries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LibraryDTO>>> GetLibraries()
        {
            return await _context.Libraries
                .Select(x => LibraryDTO(x))
                .ToListAsync();
        }

        // Returns library based on incoming ID
        [HttpGet("{id}")]
        public async Task<ActionResult<LibraryDTO>> GetLibrary(long id)
        {
            var library = await _context.Libraries.FindAsync(id);

            if (library == null)
            {
                return NotFound();
            }

            return LibraryDTO(library);
        }

        // Updates library based on incoming ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLibrary(long id, LibraryDTO libraryDTO)
        {
            if (id != libraryDTO.Id)
            {
                return BadRequest();
            }

            var library = await _context.Libraries.FindAsync(id);
            if (library == null)
            {
                return NotFound();
            }

            library.Id = libraryDTO.Id;
            library.Address = libraryDTO.Address;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!LibraryExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // Creates library and adds to library list
        [HttpPost]
        public async Task<ActionResult<Library>> CreateTodoItem(LibraryDTO libraryDTO)
        {
            var library = new Library
            {
                Address = libraryDTO.Address
            };

            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetLibrary),
                new { id = library.Id },
                LibraryDTO(library));
        }

        // Removes library from db based on incoming ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLibrary(long id)
        {
            var library = await _context.Libraries.FindAsync(id);

            if (library == null)
            {
                return NotFound();
            }

            _context.Libraries.Remove(library);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Creates instance of LibraryDTO from incoming library model
        private static LibraryDTO LibraryDTO(Library library) =>
            new LibraryDTO
            {
                Id = library.Id,
                Address = library.Address,
            };

        // Check if library exists
        private bool LibraryExists(long id) =>
            _context.Libraries.Any(e => e.Id == id);
    }
}
