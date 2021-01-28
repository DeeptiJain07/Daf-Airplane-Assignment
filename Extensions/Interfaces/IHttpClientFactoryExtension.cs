using System.Threading.Tasks;
using DAF.AirplaneTrafficData.HelperClasses;

namespace DAF.AirplaneTrafficData.Extensions.Interfaces
{
    public interface IHttpClientFactoryExtension
    {
        Task<T> CreateEndpointTask<T>(HttpEndpointTypes httpEndpointTypes);
        Task<T> GetResponseFromEndpointTask<T>(HttpEndpointTypes httpEndpointTypes);
        Task<T> UpdateEndpointTask<T>(HttpEndpointTypes httpEndpointTypes);
    }
}