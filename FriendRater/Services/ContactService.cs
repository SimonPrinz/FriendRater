using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.ContactService;
using Plugin.ContactService.Shared;

namespace FriendRater.Services
{
    public interface IPhonebookService
    {
        public Task<List<string>> GetPhoneNumbersAsync();
    }
    
    public class PhonebookService : IPhonebookService
    {
        private readonly IContactService _ContactService;
        
        public PhonebookService()
        {
            if (CrossContactService.IsSupported)
                _ContactService = CrossContactService.Current;
        }
        
        public async Task<List<string>> GetPhoneNumbersAsync()
        {
            if (_ContactService == null)
                return null;

            List<string> lNumbers = new List<string>();
            IList<Contact> lContacts = await _ContactService.GetContactListAsync();
            
            foreach (Contact lContact in lContacts)
                if (lContact.Numbers?.Count > 0)
                    lNumbers.AddRange(lContact.Numbers);
                else
                    lNumbers.Add(lContact.Number);

            return lNumbers.Distinct().ToList();
        }
    }
}