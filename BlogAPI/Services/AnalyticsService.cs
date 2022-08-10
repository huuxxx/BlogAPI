using BlogAPI.DTO;
using BlogAPI.Entities;
using BlogAPI.Interfaces;
using BlogAPI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private const int DaysInWeek = 7;
        private const string AsciiTick = "\u2713";
        private readonly VisitorContext context;

        public AnalyticsService(VisitorContext context)
        {
            this.context = context;
        }

        public Task<List<VisitorItemDTO>> GetLastVisits(int numOfRecords)
        {
            var query = context.VisitorItem.OrderByDescending(x => x.Id).Take(numOfRecords);
            List<Visitor> queryResults = query.ToList();
            queryResults.Reverse();
            List<VisitorItemDTO> retVal = new();

            for (int i = 0; i < queryResults.Count; i++)
            {
                VisitorItemDTO tempItem = new();
                tempItem.VisitorIP = queryResults[i].VisitorIP.ToString();
                tempItem.DateVisited = queryResults[i].DateVisited.ToString("dd/MM/yyyy");
                tempItem.ScreenHeight = queryResults[i].ScreenHeight.ToString();
                tempItem.ScreenWidth = queryResults[i].ScreenWidth.ToString();
                tempItem.ViewedAbout = queryResults[i].ViewedAbout ? AsciiTick : "";
                tempItem.ViewedBlogs = queryResults[i].ViewedBlogs ? AsciiTick : "";
                tempItem.ViewedProjects = queryResults[i].ViewedProjects ? AsciiTick : "";
                retVal.Add(tempItem);
            }

            return Task.FromResult(retVal);
        }

        public Task<AnalyticsOverviewDTO> GetVisitorsCount()
        {
            AnalyticsOverviewDTO overview = new();
            overview.TotalVisits = context.VisitorItem.Count();
            return Task.FromResult(overview);
        }

        public Task<AnalyticsVisitsInDayDTO[]> GetVisitorsForEachDayThisWeek()
        {
            // TODO: this would be much better stored as a cron job for the last 6 days, then append the current day

            var retVal = new List<AnalyticsVisitsInDayDTO>();
            var cultureInfo = new CultureInfo("en-US");

            for (int i = DaysInWeek; i > 0; i--)
            {
                AnalyticsVisitsInDayDTO tempDbObj = new();
                tempDbObj.NameOfDay = DateTime.Now.AddDays(-(i - 1)).DayOfWeek.ToString();
                var dateSelector = DateTime.ParseExact(DateTime.Now.AddDays(-(i - 1)).ToString("yyyy/MM/dd"), "yyyy/MM/dd", cultureInfo);
                var query = from x in context.VisitorItem
                            where x.DateVisited == dateSelector
                            select x.DateVisited;
                tempDbObj.VisitsInDay = query.Count();
                retVal.Add(tempDbObj);
            }

            return Task.FromResult(retVal.ToArray());
        }

        public async Task<int> RegisterPageView(PageViewed pageViewedItem)
        {
            var visitorItem = new Visitor { Id = pageViewedItem.SessionId };
            context.VisitorItem.Attach(visitorItem);

            switch (pageViewedItem.PageType)
            {
                case "Blogs":
                    visitorItem.ViewedBlogs = true;
                    context.Entry(visitorItem).Property(x => x.ViewedBlogs).IsModified = true;
                    break;
                case "Projects":
                    visitorItem.ViewedProjects = true;
                    context.Entry(visitorItem).Property(x => x.ViewedProjects).IsModified = true;
                    break;
                case "About":
                    visitorItem.ViewedAbout = true;
                    context.Entry(visitorItem).Property(x => x.ViewedAbout).IsModified = true;
                    break;
                default:
                    return 0;
            }

            return await context.SaveChangesAsync();
        }

        public async Task<int> RegisterNewVisitor(Visitor visitorItem, string visitorIpAddress)
        {
            visitorItem.VisitorIP = visitorIpAddress;
            context.VisitorItem.Add(visitorItem);
            await context.SaveChangesAsync();
            var newVisitorItem = context.VisitorItem.OrderByDescending(x => x.Id).FirstOrDefault();
            int newId = newVisitorItem.Id;
            return newId;
        }
    }
}
