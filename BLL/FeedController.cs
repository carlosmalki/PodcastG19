using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using PodcastAppG19.DAL;
using PodcastAppG19.Models;

namespace PodcastAppG19.BLL
{
    public class FeedController
    {
        public RepositoryFeed feedRepos;

        public Serializer newSerailzer;
       

        public   FeedController()
        {
           feedRepos = new RepositoryFeed();
           newSerailzer = new Serializer();
        }

        public List<Feed> Getallapodcast()
        {
            return feedRepos.GetAll();
        }

        public void DeleteOnCategory(Category cat)
        {
           feedRepos.DeletePodcastOnCategory(cat);
        }

        public void Create(Feed feed) 
        {
            feedRepos.Insert(feed);
        }

        public void UpdateFeedCategory(int oldCategoryIndex, Category newCategory)
        {
            feedRepos.UpdateFeedCategory(oldCategoryIndex, newCategory);
        }

        public void DeleteFeedAndContents(Feed feed)
        {
            feedRepos.DeleteFeedAndContents(feed);
        }

        public void UpdateFeedName(Feed feed, string newFeedName)
        {
            // Update the feed's name in your data storage (e.g., using RepositoryFeed)
            feedRepos.UpdateFeedName(feed, newFeedName);

            // Update the feed's name in your data structures
            feed.name = newFeedName;
        }

        public void FeedSerailizer(List<Feed> feeds)
        {
            newSerailzer.Serialize(feeds);
        }
    }
}
