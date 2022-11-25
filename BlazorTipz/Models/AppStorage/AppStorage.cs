using BlazorTipz.ViewModels;

namespace BlazorTipz.Models.AppStorage
{
    public class AppStorage : IAppStorage
    {
        public List<CategoryEntity> Categories { get; } = new List<CategoryEntity>();

        public AppStorage() 
        {
            LoadCategoriList();
        }
        private void LoadCategoriList()
        {
            Categories.Add(new CategoryEntity("1", "HMS"));
            Categories.Add(new CategoryEntity("2", "Kvalitet"));
            Categories.Add(new CategoryEntity("3", "Ledetid"));
            Categories.Add(new CategoryEntity("4", "Kostnader"));
            Categories.Add(new CategoryEntity("5", "Effektivisering"));
            Categories.Add(new CategoryEntity("6", "Kompetanse"));
            Categories.Add(new CategoryEntity("7", "Kommunikasjon"));
            Categories.Add(new CategoryEntity("8", "5S"));
            Categories.Add(new CategoryEntity("9", "Standardisering"));
            Categories.Add(new CategoryEntity("10", "Flyt"));
            Categories.Add(new CategoryEntity("11", "Visualisering"));
            Categories.Add(new CategoryEntity("12", "Energi"));
            Categories.Add(new CategoryEntity("13", "Bærekraft"));
            Categories.Add(new CategoryEntity("14", "Industri 4.0"));
        }

        public List<CategoryEntity> GetCategories()
        {
            return Categories;
        }
    }

    
    
}
