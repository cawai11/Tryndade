namespace Tryndade.Services
{
    public interface IApiService
    {
        Task<string> LoginAsync(string username, string password);
        // Adicione outros métodos conforme necessário
    }
}
