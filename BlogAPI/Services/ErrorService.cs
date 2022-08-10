using BlogAPI.DTO;
using BlogAPI.Entities;
using BlogAPI.Interfaces;
using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Services
{
    public class ErrorService : IErrorService
    {
        private readonly ErrorContext context;
        private const string TruncateString = "TRUNCATE TABLE [ErrorItem]";

        public ErrorService(ErrorContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<ErrorItem>> GetAllErrors()
        {
            var errorList = context.ErrorItem.ToList();
            List<ErrorItem> returnList = new();

            if (errorList.Count > 0)
            {
                foreach (var item in errorList)
                {
                    ErrorItem temp = new();
                    temp.Id = item.Id;
                    temp.DateCreated = item.DateCreated.ToString("dd MMM yyyy hh:mmtt");
                    temp.StackTrace = item.StackTrace;
                    temp.Message = item.Message;
                    returnList.Add(temp);
                }
            }

            return await Task.FromResult(returnList.ToArray());
        }

        public async Task<int> DeleteAllErrors()
        {
            return await Task.FromResult(context.Database.ExecuteSqlRaw(TruncateString));
        }

        public async Task<int> PostError(ErrorItemDTO errorItem)
        {
            context.ErrorItem.Add(errorItem);
            return await context.SaveChangesAsync();
        }
    }
}
