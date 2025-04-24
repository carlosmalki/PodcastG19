using PodcastAppG19.DAL;
using PodcastAppG19.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastAppG19.BLL
{
    public class CategoryController
    {
        public RepositoryCategory catRepos;

        public CategoryController()
        {
            catRepos = new RepositoryCategory();
        }

        public void UppdateKatagory(string newTitle, string oldTitle)
        {
            catRepos.UpdateCategoryByName(newTitle, oldTitle);
        }

        public void Create(String name)
        {
            Category category = new Category(name);
            catRepos.CreateCategory(category);
        }

        public List<Category> GetallaCatagory()
        {
            return catRepos.GetAll();
        }

        public Category GetCategoryByName(string name)
        {
            return catRepos.GetCategoryByName(name);
        }


        public void RemoveCategory(string category)
        {
            catRepos.DeleteCategoryByName(category);
        }

        public void Remove(Category category)
        {
            catRepos.Remove(category);
        }
    }
}
