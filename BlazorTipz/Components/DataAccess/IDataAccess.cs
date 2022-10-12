namespace BlazorTipz.Components.DataAccess
{
    public interface IDataAccess
    {
        Task<List<T>> LoadData<T, U>(string sql, U parameters, string connectionString);
        Task SaveData<T>(string sql, T parameters, string connectionString);
        Task<T> LoadData<T, U>(string sql, U parameters, string connectionString, bool isSingle);
    }
}