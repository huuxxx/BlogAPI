using BlogAPI.DTO;
using BlogAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogAPI.Interfaces
{
    public interface IErrorService
    {
        Task<IEnumerable<ErrorDto>> GetAllErrors();

        Task<int> DeleteAllErrors();

        Task<int> PostError(Error errorItem);
    }
}
