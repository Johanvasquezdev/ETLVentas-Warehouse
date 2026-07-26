using System.Threading.Tasks;
using ETLVentas.DW.application.Models.Results;

namespace ETLVentas.DW.application.Interfaces.Services
{
    /// <summary>
    /// Orquestador principal del proceso ETL (SRP: solo coordina el flujo).
    /// </summary>
    public interface IEtlOrchestratorService
    {
        Task<OperationResult> CargarDimensionesAsync();
        Task<OperationResult> CargarFactSalesAsync();
    }
}
