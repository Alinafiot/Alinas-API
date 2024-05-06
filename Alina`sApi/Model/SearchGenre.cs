using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Alina_sApi.Model
{
    public class SearchGende
    {
        public class GoogleBooksResponse
        {
            public List<Item> Items { get; set; }
        }

        public class Item
        {
            public VolumeInfo VolumeInfo { get; set; }
        }

        public class VolumeInfo
        {
            public string Title { get; set; }
            public List<string> Authors { get; set; }
        }

        public class BookItem
        {
            public string Title { get; set; }
            public List<string> Authors { get; set; }
        }
    }
}
