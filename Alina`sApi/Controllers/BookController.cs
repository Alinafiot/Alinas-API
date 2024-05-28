
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Alina_sApi.Model;
using Newtonsoft.Json;
using System.Text;

namespace Alina_sApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> _logger;

        public BookController(ILogger<BookController> logger)
        {
            _logger = logger;
        }

        [HttpGet("genre", Name = "SearchBooksByGenre")]
        public async Task<ActionResult<string>> GetByGenre(string genre, int count)
        {
            if (count > 50)
            {
                return BadRequest("Максимальна кількість книг для виведення - 50.");
            }

            if (string.IsNullOrEmpty(genre))
            {
                return BadRequest("Назва жанру не може бути порожньою.");
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://www.googleapis.com/books/v1/volumes?q=subject:{genre}&maxResults={count}");

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var books = JsonConvert.DeserializeObject<Search.GoogleBooksResponse>(responseBody);

                    // Формуємо рядок для кожної книги
                    var bookInfo = new StringBuilder();
                    foreach (var item in books.Items)
                    {
                        var authors = item.VolumeInfo.Authors != null ? string.Join(", ", item.VolumeInfo.Authors) : "Невідомий автор";
                        var title = item.VolumeInfo.Title ?? "Невідома назва";
                        bookInfo.AppendLine($"Автор(и): {authors}");
                        bookInfo.AppendLine($"Назва: {title}");
                        bookInfo.AppendLine(); // Додатковий рядок для розділення книг
                    }

                    return Ok(bookInfo.ToString());
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Помилка HTTP-запиту: {e.Message}");
                return StatusCode(500, "Помилка при обробці запиту. Будь ласка, спробуйте ще раз пізніше.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка: {ex.Message}");
                return StatusCode(500, "Внутрішня помилка сервера. Будь ласка, спробуйте ще раз пізніше.");
            }
        }


        [HttpGet("title", Name = "SearchBooksByTitle")]
        public async Task<ActionResult<string>> GetByTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("Назва книги не може бути порожньою.");
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://www.googleapis.com/books/v1/volumes?q=intitle:{title}");

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var books = JsonConvert.DeserializeObject<Search.GoogleBooksResponse>(responseBody);

                    // Формуємо рядок для книг
                    var bookInfo = new StringBuilder();
                    var counter = 0;
                    foreach (var item in books.Items)
                    {
                        var authors = item.VolumeInfo.Authors != null ? string.Join(", ", item.VolumeInfo.Authors) : "Невідомий автор";
                        var bookTitle = item.VolumeInfo.Title ?? "Невідома назва";
                        bookInfo.AppendLine($"Назва: {bookTitle}");
                        bookInfo.AppendLine($"Автор(и): {authors}");
                        bookInfo.AppendLine();

                        counter++;
                        if (counter >= 20) // Виводимо максимум 20 книг
                            break;
                    }

                    return Ok(bookInfo.ToString());
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Помилка HTTP-запиту: {e.Message}");
                return StatusCode(500, "Помилка при обробці запиту. Будь ласка, спробуйте ще раз пізніше.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка: {ex.Message}");
                return StatusCode(500, "Внутрішня помилка сервера. Будь ласка, спробуйте ще раз пізніше.");
            }
        }


        [HttpGet("author", Name = "SearchBooksByAuthor")]
        public async Task<ActionResult<string>> GetByAuthor(string author)
        {
            if (string.IsNullOrEmpty(author))
            {
                return BadRequest("Ім'я автора не може бути порожнім.");
            }

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync($"https://www.googleapis.com/books/v1/volumes?q=inauthor:{author}");

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var books = JsonConvert.DeserializeObject<Search.GoogleBooksResponse>(responseBody);

                    // Формуємо рядок для книг
                    var bookInfo = new StringBuilder();
                    var counter = 0;
                    foreach (var item in books.Items)
                    {
                        var authors = item.VolumeInfo.Authors != null ? string.Join(", ", item.VolumeInfo.Authors) : "Невідомий автор";
                        var bookTitle = item.VolumeInfo.Title ?? "Невідома назва";
                        bookInfo.AppendLine($"Автор(и): {authors}");
                        bookInfo.AppendLine($"Назва: {bookTitle}");
                        bookInfo.AppendLine();

                        counter++;
                        if (counter >= 20) // Виводимо максимум 20 книг
                            break;
                    }

                    return Ok(bookInfo.ToString());
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Помилка HTTP-запиту: {e.Message}");
                return StatusCode(500, "Помилка при обробці запиту. Будь ласка, спробуйте ще раз пізніше.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка: {ex.Message}");
                return StatusCode(500, "Внутрішня помилка сервера. Будь ласка, спробуйте ще раз пізніше.");
            }
        }


        [HttpGet("randomBooks", Name = "GetRandomBooks")]
        public async Task<ActionResult<string>> GetRandomBooks()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://www.googleapis.com/books/v1/volumes?q=random&maxResults=10");

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var books = JsonConvert.DeserializeObject<Search.GoogleBooksResponse>(responseBody);

                    // Формуємо рядок для книг
                    var bookInfo = new StringBuilder();
                    bookInfo.AppendLine("10 випадкових книг:");
                    foreach (var item in books.Items)
                    {
                        var authors = item.VolumeInfo.Authors != null ? string.Join(", ", item.VolumeInfo.Authors) : "Невідомий автор";
                        var bookTitle = item.VolumeInfo.Title ?? "Невідома назва";
                        bookInfo.AppendLine($"Автор(и): {authors}");
                        bookInfo.AppendLine($"Назва: {bookTitle}");
                        bookInfo.AppendLine();
                    }

                    return Ok(bookInfo.ToString());
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Помилка HTTP-запиту: {e.Message}");
                return StatusCode(500, "Помилка при обробці запиту. Будь ласка, спробуйте ще раз пізніше.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка: {ex.Message}");
                return StatusCode(500, "Внутрішня помилка сервера. Будь ласка, спробуйте ще раз пізніше.");
            }
        }

        [HttpPost]



        private bool IsValidQuery(string query)
        {
            return true;
        }
    }
}

