using BlazorTipz.Models;

namespace BlazorTipz.ViewModels
{
    public class Category
    {
        public string Name { get; set; }

        public Category() { }
        public Category(CategoriEntity cat)
        {
            this.Name = cat.catName;
        }
        //Override ToString
        public override string ToString()
        {
            return Name;
        }
    }
}
