using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreWebAppSample2.RequestResponceLogging
{
    public interface IResponceRequestLogger
    {
        void LogMessage(string request, string responce);
        List<ResponceRequestLogItem> GetAllMessages();
    }

    public class ResponceRequestLoggerDefaultImpl : IResponceRequestLogger
    {
        private static ConcurrentBag<ResponceRequestLogItem> _logStorage = new ConcurrentBag<ResponceRequestLogItem>();

        public void LogMessage(string request, string responce)
        {
            _logStorage.Add(new ResponceRequestLogItem() { Request = request, Responce = responce, ActionDate = DateTime.Now });
        }

        public List<ResponceRequestLogItem> GetAllMessages()
        {
            var res = _logStorage.ToList();
            return res;
        }


    }

    public class ResponceRequestLogItem
    {
        public string Request { get; set; }

        public string Responce { get; set; }

        public DateTime ActionDate { get; set; }
    }
}
