using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private HttpClient _httpClient;
        private readonly string _userServiceUrl = "http://127.0.0.1:5001";

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<int> CheckOrCreateAsync(string phone)
        {
            var data = new Dictionary<string, string>
            {
                ["phone"] = phone
            };
            var content = new FormUrlEncodedContent(data);
            var url = $"{_userServiceUrl}/api/Users/checkorcreate";
            var response = await _httpClient.PostAsync(url, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var userId = await response.Content.ReadAsStringAsync();
                int.TryParse(userId, out int intUserId);
                return intUserId;
            }
            return 0;
        }
    }
}
