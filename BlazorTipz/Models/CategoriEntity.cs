using BlazorTipz.ViewModels;

namespace BlazorTipz.Models
{
    public class CategoriEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public CategoriEntity(string id, string name)
        {
            Id = id;
            Name = name;
        }
        public CategoriEntity()
        {
        }
        public CategoriEntity(Category cat)
        {
            this.Name = cat.Name;  
        }
    }
}
