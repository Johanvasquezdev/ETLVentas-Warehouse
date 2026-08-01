using System.Threading.Tasks;
using ETLVentas.DW.application.Models.Results;

namespace ETLVentas.DW.application.Interfaces.Services
{
    public interface IEtlOrchestratorService
    {
        Task<OperationResult> CargarDimensionesAsync();
        Task<OperationResult> CargarFactSalesAsync();
    }
}
