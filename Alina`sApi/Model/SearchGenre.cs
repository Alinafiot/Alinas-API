namespace Alina_sApi.Model
{
    public class Search
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
    }

    public class ReviewRequest
    {
        public string BookName { get; set; }
        public int Rating { get; set; }
    }

}
