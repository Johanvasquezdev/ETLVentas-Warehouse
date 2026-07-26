using System.Threading.Tasks;
using ETLVentas.DW.application.Models.Results;

namespace ETLVentas.DW.application.Interfaces.Services
{
    /// <summary>
    /// Servicio encargado de limpiar las tablas del DW antes de cada ejecución (SRP).
    /// </summary>
    public interface IDbCleanupService
    {
        Task<OperationResult> CleanDimensionsAsync();
        Task<OperationResult> CleanFactsAsync();
    }
}
