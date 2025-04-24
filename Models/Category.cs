using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastAppG19.Models
{

    public class Category
    {

        public string Title { get; set; }

        public Category(string name)
        {
            Title = name;
        }

        public Category() { }

        public void ChangeTitle(string newTitle)
        {
            Title = newTitle;
        }
    }
}
