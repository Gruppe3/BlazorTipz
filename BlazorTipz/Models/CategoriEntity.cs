using BlazorTipz.ViewModels;

namespace BlazorTipz.Models
{
    public class CategoriEntity
    {
        public string catId { get; set; }
        public string catName { get; set; }

        public CategoriEntity(string id, string name)
        {
            this.catId = id;
            this.catName = name;
        }
        public CategoriEntity()
        {
        }
        
        public CategoriEntity(Category cat)
        {
            this.catName = cat.Name;  
        }
    }
}
