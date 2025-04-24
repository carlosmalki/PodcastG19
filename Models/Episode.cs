using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastAppG19.Models
{
    public class Episode
    {
        public string name { get; set; }
        public string description { get; set; }


        public Episode(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public Episode() { }
    }
}
