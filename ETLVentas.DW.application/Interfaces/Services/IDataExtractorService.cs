using System.Collections.Generic;
using System.Threading.Tasks;
using ETLVentas.DW.application.Models.Dtos;

namespace ETLVentas.DW.application.Interfaces.Services
{
    public interface IDataExtractorService
    {
        string SourceName { get; }
        Task<IEnumerable<VentaExtraidaDto>> ExtractAsync();
    }
}
