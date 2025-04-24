
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using PodcastAppG19.DAL;

namespace PodcastAppG19.Models
{
    [DataContract]
    public class Feed
    {
        //  private Serializer serializer;
        private Serializer se;
        public string name { get; set; }

        public string url { get; set; }

        public int frequency { get; set; }

        public Category category { get; set; }

        private RepositoryFeed repositoryFeed;

        public List<Episode> episodes { get; set; }

        public Feed(string name, string url, int frequency, Category category)
        {

            se = new Serializer();
            this.name = name;
            this.url = url;
            this.category = category;
            this.frequency = frequency;
            repositoryFeed = new RepositoryFeed();
            episodes = new List<Episode>();
            GetEpisodes();
        }

        public Feed() { }

        public async Task<string> GetFeedTitleAsync()
        {
            repositoryFeed = new RepositoryFeed();
            se = new Serializer();
            try
            {
                string title = await repositoryFeed.GetFeedTitleAsync(url);
                return title;
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        public int GetEpisodeNumber()
        {
            int EpisodeNumber = episodes.Count();
            return EpisodeNumber;
        }

        public void GetEpisodes()
        {
            episodes = repositoryFeed.GetEpisodes(url);
        }
    }
}
