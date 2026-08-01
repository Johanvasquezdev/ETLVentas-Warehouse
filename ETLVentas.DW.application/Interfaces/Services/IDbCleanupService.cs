using System.Threading.Tasks;
using ETLVentas.DW.application.Models.Results;

namespace ETLVentas.DW.application.Interfaces.Services
{
    public interface IDbCleanupService
    {
        Task<OperationResult> CleanDimensionsAsync();
        Task<OperationResult> CleanFactsAsync();
    }
}
