using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FriendRater.Api;
using FriendRater.Api.Models;
using FriendRater.Data;

namespace FriendRater.Services
{
    public interface IApiService
    {
        public Task<bool> IsLoggedInAsync();

        public Task<bool> LoginAsync(string pUsername, string pPassword, bool pRememberMe = false);

        public Task<(bool Success, Exception Error)> RegisterAsync(RegisterRequest pRequest);

        public Task<List<User>> GetUsersInContacts();

        public Task<List<User>> Search(string pSearchTerm);

        public string GetName();

        public Task<UserProfile> LoadUserProfile(Guid pGuid);

        public Task LogoutAsync();
    }
    
    public class ApiService : IApiService
    {
        private readonly IOptionService _OptionService;
        private readonly IPhonebookService _PhonebookService;

        private ApiState _State;
        
        public ApiService(IOptionService pOptionService, IPhonebookService pPhonebookService)
        {
            _OptionService = pOptionService;
            _PhonebookService = pPhonebookService;
        }
        
        public async Task<bool> IsLoggedInAsync()
        {
            if (_State != null)
                return true;

            Credentials lCredentials = await _OptionService.GetOptionAsync<Credentials>(nameof(Credentials));
            if (lCredentials == null)
                return false;

            ApiState lState = await CheckCredentials(lCredentials);
            if (lState == null)
                return false;
            
            _State = lState;
            return true;
        }

        public async Task<bool> LoginAsync(string pUsername, string pPassword, bool pRememberMe = false)
        {
            if (string.IsNullOrWhiteSpace(pUsername) ||
                string.IsNullOrWhiteSpace(pPassword))
                return false;

            Credentials lCredentials = new Credentials
            {
                Username = pUsername,
                Password = pPassword
            };
            ApiState lState = await CheckCredentials(lCredentials);
            if (lState == null)
                return false;
            
            if (pRememberMe)
                await _OptionService.SetOptionAsync(nameof(Credentials), lCredentials);
            else
                await _OptionService.DeleteOptionAsync(nameof(Credentials));

            _State = lState;
            return true;
        }

        public async Task<(bool Success, Exception Error)> RegisterAsync(RegisterRequest pRequest)
        {
            try
            {
                using ApiClient lClient = new ApiClient(App.API);
                return (await lClient.Register(pRequest), null);
            }
            catch (Exception lException)
            {
                return (false, lException);
            }
        }

        public async Task<List<User>> GetUsersInContacts()
        {
            if (_State == null)
                return null;
            
            List<string> lNumbers = await _PhonebookService.GetPhoneNumbersAsync();
            if (lNumbers == null || !lNumbers.Any())
                return new List<User>();

            try
            {
                using ApiClient lClient = new ApiClient(_State);
                return await lClient.NumberSearch(lNumbers);
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }

        public async Task<List<User>> Search(string pSearchTerm)
        {
            if (_State == null || string.IsNullOrWhiteSpace(pSearchTerm))
                return null;

            try
            {
                using ApiClient lClient = new ApiClient(_State);
                return await lClient.Search(pSearchTerm);
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }

        public string GetName() => _State?.Name;

        public async Task<UserProfile> LoadUserProfile(Guid pGuid)
        {
            if (_State == null || pGuid == Guid.Empty)
                return null;

            try
            {
                using ApiClient lClient = new ApiClient(_State);
                return await lClient.Profile(pGuid);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            _State = null;
            await _OptionService.ClearOptionsAsync();
        }

        private static async Task<ApiState> CheckCredentials(Credentials pCredentials)
        {
            if (pCredentials == null)
                return null;

            try
            {
                using ApiClient lClient = new ApiClient(App.API);
                await lClient.Login(pCredentials.Username, pCredentials.Password);
                return lClient.State;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}