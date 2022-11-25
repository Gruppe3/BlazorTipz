using BlazorTipz.ViewModels;

namespace BlazorTipz.Models
{
    public class CategoryEntity
    {
        public string catId { get; set; } = string.Empty;
        public string catName { get; set; } = string.Empty;

        
        public CategoryEntity()
        {
        }
        
        public CategoryEntity(string id, string name)
        {
            this.catId = id;
            this.catName = name;
        }
        
        public CategoryEntity(Category cat)
        {
            this.catId = cat.Id;
            this.catName = cat.Name; 
        }
    }
}
