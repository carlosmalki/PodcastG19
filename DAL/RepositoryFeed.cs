using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using PodcastAppG19.ExceptionHandling;
using System.Net;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Reflection;
using PodcastAppG19.Models;

namespace PodcastAppG19.DAL
{
    public class RepositoryFeed : FeedIFRepository<Feed>
    {
        Serializer se = new Serializer();
        List<Feed> list;

        public RepositoryFeed()
        {
            list = GetAll();
        }

        public void Insert(Feed entity)
        {
            list.Add(entity);
            Save();
        }

        public void Save()
        {
            se.Serialize(list);
        }

        public List<Feed> GetAll()
        {
           return  se.DeserializeFeed();
        }


        public void Delete(Feed t)
        {
            list.Remove(t);
            Save();
        }

        public void Update(Feed t)
        {
            // Create a list to store updated podcasts
            List<Feed> updatedPodcasts = new List<Feed>();

            // Iterate through the existing podcasts and remove those with the same name as the new podcast
            foreach (var pod in list)
            {
                if (pod.name != t.name)
                {
                    updatedPodcasts.Add(pod);
                }
            }

            // Replace the old list with the updated list
            list = updatedPodcasts;

            // Add the new podcast
            Insert(t);

            // Retrieve all data
            GetAll();
        }

        public async Task<string> GetFeedTitleAsync(string url)
        {
            try
            {
                if (url.Contains("feed", StringComparison.OrdinalIgnoreCase)
                     || url.Contains("pod", StringComparison.OrdinalIgnoreCase)) 
                { 
                    XDocument file = XDocument.Load(url);
                    var firstTitle = Task.Run(() => file.Descendants("title").First()).Result;
                    string title = firstTitle.Value;
                    return title;
                }
                else
                {
                    throw new UrlException(new XDocument(), "Felaktig URL");
                }
            }
            catch (UrlException ex)
            {
                // Observera: Här används normalt en Exception-loggning eller annan hantering,
                // eftersom MessageBox inte är tillgänglig utanför UI-tråden.
                // Logga ex med lämplig mekanism eller använd lämplig hantering för applikationen.
                Console.WriteLine(ex.Message);
                return null!;
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("URL ej hittad, försök igen!");
                return null!;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception klass :(" + ex.Message);
                return null!;
            }
        }

        public List<Episode> GetEpisodes(string url) 
        {
            List<Episode> episodeList = new List<Episode>();

            try
            {
                if (url.Contains("feed", StringComparison.OrdinalIgnoreCase)
                    || url.Contains("pod", StringComparison.OrdinalIgnoreCase))
                {
                    XDocument filen = XDocument.Load(url);
                    List<XElement> descriptionList = filen.Descendants("description").ToList();
                    List<XElement> titleList = filen.Descendants("title").ToList(); 
                    int index = 0;

                    foreach (XElement item in titleList)
                    {
                        string title = (string)item;
                        if (title.Contains("."))
                        {
                            episodeList.Add(new Episode(title, (string)descriptionList.ElementAt(index)));
                            index++;

                        }
                    }
                    return episodeList;
                }
                else
                    //Kasta ett anpassat undantag med felmeddelandet om URL:en inte har rätt format
                    throw new UrlException(new XDocument(), "Denna länk är ogiltig eller leder inte till en prenumererbar ljud- eller videofeed");
            }
            catch
            {
                return episodeList;
            }
        }

        public void UpdateFeedCategory(int oldCategoryIndex, Category newCategory)
        {
            list.ElementAt(oldCategoryIndex).category = newCategory;
            Save();
        }

        public void DeleteFeedAndContents(Feed feed)
        {
            foreach(Feed f in list) 
            { 
                if(f.url == feed.url) 
                { 
                    list.Remove(f);
                    break;
                }
            }

            // Optionally, delete any associated content or perform cleanup here

            Save(); // Save the updated list
        }

        public void DeletePodcastOnCategory(Category cat)
        {
            list.RemoveAll(podcast => podcast.category.Title == cat.Title);
            Save();
            GetAll();
        }

        public void UpdateFeedName(Feed feed, string newFeedName)
        {
            foreach (Feed f in list)
            {
                if (f.name == feed.name)
                {
                    f.name = newFeedName;
                    Save(); // Save the updated feed list
                    break;
                }
            }
        }
    }




}
