using System.Collections.Generic;
using System.Threading.Tasks;
using CESI.Handlers;
using CESI.ResponseModels;

namespace CESI.APIs
{
    public class AssetEndpoints
    {
        private readonly CesiHttpHandler _cesiHttpHandler;

        public AssetEndpoints(CesiHttpHandler cesiHttpHandler)
        {
            _cesiHttpHandler = cesiHttpHandler;
        }

        public async Task<IEnumerable<AssetResponse>> GetCharacterAssetsAsync(int characterId, int page = 1, string ifNoneMatch = null)
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "page", page.ToString() }
            };
            
            var response = await _cesiHttpHandler.GetAsync<IEnumerable<AssetResponse>>($"/v5/characters/{characterId}/assets/", ifNoneMatch, queryParameters);

            return response.Model;
        }
    }
}