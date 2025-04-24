using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastAppG19.DAL 
{
    internal interface FeedIFRepository<T> where T : class
    {
        public void Delete(T t);
        public void Update(T t);
        public void Insert(T t);
        public void Save();
        public List<T> GetAll();
    }
}
    

