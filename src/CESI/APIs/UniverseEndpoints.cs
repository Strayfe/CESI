using System.Threading.Tasks;
using CESI.Handlers;
using CESI.ResponseModels;

namespace CESI.APIs
{
    public class UniverseEndpoints
    {
        private readonly CesiHttpHandler _cesiHttpHandler;

        public UniverseEndpoints(CesiHttpHandler cesiHttpHandler)
        {
            _cesiHttpHandler = cesiHttpHandler;
        }

        public async Task<StructureResponse> GetStructureByIdAsync(long structureId, string ifNoneMatch = null)
        {
            var response = await _cesiHttpHandler.GetAsync<StructureResponse>($"/v2/universe/structures/{structureId}/", ifNoneMatch);

            return response.Model;
        }

        public async Task<SystemResponse> GetSolarSystemByIdAsync(long solarSystemId, string ifNoneMatch = null)
        {
            var response = await _cesiHttpHandler.GetAsync<SystemResponse>($"/v4/universe/systems/{solarSystemId}/", ifNoneMatch);

            return response.Model;
        }
    }
}