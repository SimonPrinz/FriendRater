using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using FriendRater.Api.Models;
using Newtonsoft.Json;

namespace FriendRater.Api
{
    public class ApiClient : IDisposable
    {
        private readonly ApiEnvironment _Environment;
        private readonly HttpClient _Client;

        public ApiClient(ApiEnvironment pEnvironment)
        {
            _Environment = pEnvironment;
            _Client = new HttpClient { BaseAddress = new Uri(_Environment.BaseUrl) };
        }

        public async Task<ApiWrapper<LoginResponse>> Login(string pUsername, string pPassword)
        {
            _Client.DefaultRequestHeaders.Authorization = null;
            AuthenticationHeaderValue lAuthorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{pUsername}:{pPassword}")));
            ApiWrapper<LoginResponse> lResponse = null;
            try
            {
                using HttpRequestMessage lRequest = new HttpRequestMessage(HttpMethod.Get, "login");
                lRequest.Headers.Authorization = lAuthorization;
                return lResponse = await SendAsync<LoginResponse>(lRequest);
            }
            finally
            {
                if (lResponse != null && lResponse.Ok)
                    _Client.DefaultRequestHeaders.Authorization = lAuthorization;
            }
        }

        public async Task<ApiWrapper<bool>> Register(RegisterRequest pRequest)
        {
            AuthenticationHeaderValue lAuth = _Client.DefaultRequestHeaders.Authorization;
            _Client.DefaultRequestHeaders.Authorization = null;
            try
            {
                string lJson = JsonConvert.SerializeObject(pRequest);
                using HttpRequestMessage lRequest = new HttpRequestMessage(HttpMethod.Post, "register");
                lRequest.Content = new StringContent(lJson, Encoding.UTF8, "application/json");
                return await SendAsync<bool>(lRequest);
            }
            finally
            {
                _Client.DefaultRequestHeaders.Authorization = lAuth;
            }
        }

        public async Task<ApiWrapper<List<User>>> Search(string pSearch)
        {
            using HttpRequestMessage lRequest = new HttpRequestMessage(HttpMethod.Get, $"search?q={HttpUtility.UrlEncode(pSearch)}");
            return await SendAsync<List<User>>(lRequest);
        }

        public async Task<ApiWrapper<List<User>>> NumberSearch(List<string> pNumbers)
        {
            string lJson = JsonConvert.SerializeObject(pNumbers);
            using HttpRequestMessage lRequest = new HttpRequestMessage(HttpMethod.Post, "numbersearch");
            lRequest.Content = new StringContent(lJson, Encoding.UTF8, "application/json");
            return await SendAsync<List<User>>(lRequest);
        }

#nullable enable
        public async Task<ApiWrapper<UserProfile>> Profile(Guid? pUserId = null)
#nullable restore
        {
            using HttpRequestMessage lRequest = new HttpRequestMessage(HttpMethod.Get, $"profile{(pUserId.HasValue ? "id=" + pUserId.Value.ToString() : "")}");
            return await SendAsync<UserProfile>(lRequest);
        }

        public async Task<ApiWrapper<List<UserComment>>> Comments(Guid pUserId)
        {
            using HttpRequestMessage lRequest = new HttpRequestMessage(HttpMethod.Get, $"profile?id={pUserId}");
            return await SendAsync<List<UserComment>>(lRequest);
        }

        private async Task<ApiWrapper<TResponse>> SendAsync<TResponse>(HttpRequestMessage pRequest) where TResponse : new()
        {
            try
            {
                using HttpResponseMessage lResponse = await _Client.SendAsync(pRequest);
                string lJson = await lResponse.Content.ReadAsStringAsync();
                ApiWrapper<TResponse> lApiResponse = JsonConvert.DeserializeObject<ApiWrapper<TResponse>>(lJson);
                if (!lApiResponse.Ok)
                    throw new ApiException("server returned an error", lApiResponse.Errors);
                return lApiResponse;
            }
            catch (Exception lException)
            {
                if (lException is ApiException)
                    throw;
                throw new ApiException(lException.Message);
            }
        }

        public void Dispose()
        {
            _Client.Dispose();
        }
    }

    public sealed class ApiEnvironment
    {
        public static ApiEnvironment v1Production = new ApiEnvironment(new Dictionary<string, object>
        {
            { nameof(Version), 1 },
            { nameof(BaseUrl), "https://friendraterapi.simonprinz.me/v1/" },
        });
        public static ApiEnvironment v1Staging = new ApiEnvironment(new Dictionary<string, object>
        {
            { nameof(Version), 1 },
            { nameof(BaseUrl), "https://test.friendraterapi.simonprinz.me/v1/" },
        });

        public int Version => Get<int>();
        public string BaseUrl => Get<string>();

        private Dictionary<string, object> _Values;

        private ApiEnvironment(Dictionary<string, object> pValues)
        {
            _Values = pValues;
        }

        public T Get<T>([CallerMemberName] string pKey = null, T pDefault = default)
        {
            if (!_Values.ContainsKey(pKey))
                return pDefault;

            return _Values.TryGetValue(pKey, out object lValue) && lValue is T lTypedValue
                ? lTypedValue
                : pDefault;
        }
    }

    public class ApiException : Exception
    {
        public Error[] Errors { get; }

        public ApiException(string pMessage, List<Error> pErrors = null) : base(pMessage, pErrors != null && pErrors.Count > 0 ? GetExceptionTrace(pErrors) : null)
        {
            Errors = pErrors.ToArray();
        }

        private static Exception GetExceptionTrace(List<Error> pErrors)
        {
            if (pErrors.Count == 0)
                return null;

            Error lError = pErrors[0];
            pErrors.RemoveAt(0);

            return new Exception(lError.Message, GetExceptionTrace(pErrors));
        }
    }
}
