using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace F24_Assignment3_mwebster.Models
{
    public class RedditService
    {
        // This method makes the Reddit API call and fetches the posts based on a search query
        public static async Task<List<string>> SearchRedditAsync(string searchQuery)
        {
            var returnList = new List<string>();
            var json = "";

            using (HttpClient client = new HttpClient())
            {
                // Pretend to be a "real" web browser
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                json = await client.GetStringAsync("https://www.reddit.com/search.json?limit=100&q=" + System.Web.HttpUtility.UrlEncode(searchQuery));
            }

            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement dataElement = doc.RootElement.GetProperty("data");
            JsonElement childrenElement = dataElement.GetProperty("children");

            foreach (JsonElement child in childrenElement.EnumerateArray())
            {
                if (child.TryGetProperty("data", out JsonElement data))
                {
                    if (data.TryGetProperty("selftext", out JsonElement selftext))
                    {
                        string selftextValue = selftext.GetString();
                        if (!string.IsNullOrEmpty(selftextValue))
                        {
                            returnList.Add(selftextValue);
                        }
                        else if (data.TryGetProperty("title", out JsonElement title)) // Use title if selftext is empty
                        {
                            string titleValue = title.GetString();
                            if (!string.IsNullOrEmpty(titleValue))
                            {
                                returnList.Add(titleValue);
                            }
                        }
                    }
                }
            }

            return returnList;
        }
    }
}
