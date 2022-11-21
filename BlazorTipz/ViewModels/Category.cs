using BlazorTipz.Models;

namespace BlazorTipz.ViewModels
{
    public class Category
    {
        public string Id { get; set; } = "0";
        public string Name { get; set; } = string.Empty;

        public Category() 
        { 
        }
        public Category(CategoryEntity cat)
        {
            this.Id = cat.catId;
            this.Name = cat.catName;
        }
        //Override ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
