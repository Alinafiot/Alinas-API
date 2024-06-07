using Npgsql;
using System.Threading.Tasks;

namespace Alina_sApi
{
    public class Database
    {
        NpgsqlConnection con = new NpgsqlConnection(Constants.Connect);

        public async Task InsertBookAsync(string title)
        {
            var sql = "insert into public.\"RatingBooks\"(\"Title\")"
                    + "values (@Title)";

            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("Title", title);

            await con.OpenAsync();
            await comm.ExecuteNonQueryAsync();
            await con.CloseAsync();
        }
        public async Task<bool> DeleteReviewForBookAsync(string title)
        {
            var sql = "delete from public.\"RatingBooks\" where \"Title\" = @Title";

            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("Title", title);

            await con.OpenAsync();
            var rowsAffected = await comm.ExecuteNonQueryAsync();
            await con.CloseAsync();

            return rowsAffected > 0;
        }

        public async Task<List<string>> GetBooksAsync()
        {
            var books = new List<string>();
            var sql = "select \"Title\" from public.\"RatingBooks\"";

            NpgsqlCommand comm = new NpgsqlCommand(sql, con);

            await con.OpenAsync();
            var reader = await comm.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                books.Add(reader.GetString(0));
            }

            await reader.CloseAsync();
            await con.CloseAsync();

            return books;
        }

    }
}
