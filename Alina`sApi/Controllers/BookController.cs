using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Alina_sApi.Model; 

namespace Alina_sApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<BookController> _logger;

        public BookController(IHttpClientFactory clientFactory, ILogger<BookController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [HttpGet(Name = "SearchBooksByGenre")]
        public async Task<ActionResult<string>> Get(string genre, int count)
        {
            if (count > 50)
            {
                return BadRequest("Максимальна кількість книг для виведення - 50.");
            }

            try
            {
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"https://www.googleapis.com/books/v1/volumes?q=subject:{genre}&maxResults={count}");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                return Ok(ParseResponse(responseBody));
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

        private string ParseResponse(string responseBody)
        {
            var books = new List<SearchGende.BookItem>(); 

            JObject jsonResponse = JObject.Parse(responseBody);
            JArray items = (JArray)jsonResponse["items"];

            StringBuilder result = new StringBuilder();
            foreach (JToken item in items)
            {
                string title = item["volumeInfo"]["title"].ToString();
                var authors = item["volumeInfo"]["authors"]?.ToObject<List<string>>() ?? new List<string>();

                result.AppendLine($"Назва: {title}");
                result.AppendLine("Автор(и):");
                foreach (var author in authors)
                {
                    result.AppendLine($"- {author}");
                }
                result.AppendLine();
            }

            return result.ToString();
        }
    }
}

