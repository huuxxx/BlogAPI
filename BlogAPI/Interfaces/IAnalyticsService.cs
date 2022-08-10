using BlogAPI.DTO;
using BlogAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogAPI.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsOverviewDTO> GetVisitorsCount();

        Task<AnalyticsVisitsInDayDTO[]> GetVisitorsForEachDayThisWeek();

        Task<List<VisitorItemDTO>> GetLastVisits(int numOfRecords);

        Task<int> RegisterNewVisitor(Visitor visitorItem, string visitorIpAddress);

        Task<int> RegisterPageView(PageViewed pageViewedItem);
    }
}
