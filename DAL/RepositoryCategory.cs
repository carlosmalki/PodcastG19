using PodcastAppG19.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastAppG19.DAL
{
    public class RepositoryCategory : CategoryIFRepository<Category>
    {
        Category cat;
        List<Category> list;
        Serializer se = new Serializer();

        public RepositoryCategory()
        {
            cat = new Category();
            list = new List<Category>();
            GetAll();
        }

        public List<Category> GetAll()
        {
            return list = se.DeserializeCategory();
        }

        public void ChangeCategory(Category newCategory)
        {
            cat = newCategory;
        }

        public void DeleteAllCategories()
        {
            throw new NotImplementedException();
        }

        public void DeleteCategoryByName(string name)
        {
            Category categoryToRemove = null!;
            
            foreach (Category category in list)
            {
                if (category.Title.Equals(name))
                {
                    categoryToRemove = category;
                    break;
                }
            }
            
            if (categoryToRemove != null)
            {
                list.Remove(categoryToRemove);
                Save();
            }
        }

        public Category GetCategoryByName(string name)
        {
            Category category = GetAll().FirstOrDefault(category => category.Title.Equals(name))!;
            return category;
        }

        public void UpdateCategoryByName(string name, string newName)
        {
            Category category = GetCategoryByName(name);
            category.ChangeTitle(newName);
            Save();
        }

        public void CreateCategory( Category category)
        {
            list.Add(category);
            Save();
            GetAll();
        }

        public void Save()
        {
            se.Serialize(list);
        }

        public void Remove(Category en)
        {
            list.Remove(en); 
            Save();
        }
    }
}
