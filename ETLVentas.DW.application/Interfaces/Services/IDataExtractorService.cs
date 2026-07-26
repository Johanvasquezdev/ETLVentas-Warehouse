using System.Collections.Generic;
using System.Threading.Tasks;
using ETLVentas.DW.application.Models.Dtos;

namespace ETLVentas.DW.application.Interfaces.Services
{
    /// <summary>
    /// Interfaz base para todos los extractores de datos (SRP: solo extrae, no transforma ni carga).
    /// </summary>
    public interface IDataExtractorService
    {
        string SourceName { get; }
        Task<IEnumerable<VentaExtraidaDto>> ExtractAsync();
    }
}
