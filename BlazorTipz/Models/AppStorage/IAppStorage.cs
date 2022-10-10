using BlazorTipz.Models.DbRelay;
using BlazorTipz.ViewModels;

namespace BlazorTipz.Models.AppStorage
{
    public interface IAppStorage
    {
        //returs a list of available categories
        List<CategoriEntity> GetCategories();
    }
}
