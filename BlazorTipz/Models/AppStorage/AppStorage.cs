using BlazorTipz.ViewModels;

namespace BlazorTipz.Models.AppStorage
{
    public class AppStorage : IAppStorage
    {
        public List<CategoriEntity> categoris { get; } = new List<CategoriEntity>();

        public AppStorage() 
        {
            loadCategoriList();
        }
        private void loadCategoriList()
        {
            categoris.Add(new CategoriEntity("1", "HMS"));
            categoris.Add(new CategoriEntity("2", "Kvalitet"));
            categoris.Add(new CategoriEntity("3", "Ledetid"));
            categoris.Add(new CategoriEntity("4", "Kostnader"));
            categoris.Add(new CategoriEntity("5", "Effektivisering"));
            categoris.Add(new CategoriEntity("6", "Kompetanse"));
            categoris.Add(new CategoriEntity("7", "Kommunikasjon"));
            categoris.Add(new CategoriEntity("8", "5S"));
            categoris.Add(new CategoriEntity("9", "Standardisering"));
            categoris.Add(new CategoriEntity("10", "Flyt"));
            categoris.Add(new CategoriEntity("11", "Visualisering"));
            categoris.Add(new CategoriEntity("12", "Energi"));
            categoris.Add(new CategoriEntity("13", "Bærekraft"));
            categoris.Add(new CategoriEntity("14", "Industri 4.0"));
        }

        public List<CategoriEntity> GetCategories()
        {
            return categoris;
        }
    }

    
    
}
