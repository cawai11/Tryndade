using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tryndade.Models.ViewModels;

namespace Tryndade.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var loginDto = new
            {
                Username = username,
                Password = password
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);
                return jsonResponse.Token;
            }
            return null;
        }

        public async Task<T> GetProtectedDataAsync<T>(string endpoint)
        {
            var accessToken = await GetAccessTokenAsync();
            if (accessToken == null)
            {
                // Tratar o caso em que o token não está disponível
                return default(T);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<T>(responseString);
                return data;
            }
            else
            {
                // Tratar erros
                return default(T);
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            // Recuperar o token dos claims
            var accessToken = _httpContextAccessor.HttpContext.User.FindFirst("AccessToken")?.Value;
            return accessToken;
        }

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            var registerDto = new
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/register", content);
            return response.IsSuccessStatusCode;
        }

    }
}
