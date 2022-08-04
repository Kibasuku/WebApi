using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using System.Text.Json;
using System.Drawing;
using System.Collections;
using System.Linq;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        
        // GET: api/<BooksController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> Get()
        {
            try
            {
                using (FileStream fs = new FileStream("books.json", FileMode.OpenOrCreate))
                {
                    var books = await JsonSerializer.DeserializeAsync<List<Book>>(fs);
                    return books;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
            
        }

        // GET api/<BooksController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(int id)
        {
            try
            {
                var books = await ReadData();

                var book = books.FirstOrDefault(x => x.Id == id);
                if (book == null)
                { return book; }
                return new ObjectResult(book);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        // POST api/<BooksController>
        [HttpPost]
        public async Task Post([FromBody] Book book)
        {
            try 
            {
                var books = await ReadData();
                books.Add(book);
                await UpdateData(books);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                StatusCode(500);
            }
            
        }

        // PUT api/<BooksController>/5
        [HttpPut]
        public async Task<ActionResult<Book>> Put([FromBody] Book book)
        {
            try
            {
                var books = await ReadData();

                if (book == null)
                {
                    return BadRequest();
                }
                if (!books.Any(x => x.Id == book.Id))
                {
                    return NotFound();
                }

                int index = books.FindIndex(x => x.Id == book.Id);
                books[index] = book;
                await UpdateData(books);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }

        // DELETE api/<BooksController>/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            try
            {
                var books = await ReadData();

                var book = books.FirstOrDefault(x => x.Id == id);
                if (book == null)
                {
                    NotFound();
                }
                books.Remove(book);
                await UpdateData(books);
                Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                StatusCode(500);
            }
        }

        private async Task UpdateData(List<Book> books)
        {
            using (FileStream fs = new FileStream("books.json", FileMode.Truncate))
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                await JsonSerializer.SerializeAsync<List<Book>>(fs, books,options);
                Console.WriteLine("Data has been saved to file");
            }
        }

        private async Task<List<Book>> ReadData()
        {
            using (FileStream fs = new FileStream("books.json", FileMode.OpenOrCreate))
            {
                var books = await JsonSerializer.DeserializeAsync<List<Book>>(fs);
                return books;
            }           

        }
    }
}
