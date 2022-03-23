using System.Threading.Tasks;
using CESI.Handlers;
using CESI.Models;
using CESI.ResponseModels;

namespace CESI.APIs
{
    public class CharacterEndpoints
    {
        private readonly CesiHttpHandler _cesiHttpHandler;
        
        public CharacterEndpoints(CesiHttpHandler cesiHttpHandler)
        {
            _cesiHttpHandler = cesiHttpHandler;
        }
        
        public async Task<IconsResponse> GetCharacterPortraitUrlsAsync(int characterId, string ifNoneMatch = null)
        {
            var response = await _cesiHttpHandler.GetAsync<IconsResponse>($"/v2/characters/{characterId}/portrait/", ifNoneMatch);

            return response.Model;
        }
    }
}