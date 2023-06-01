namespace Business.Domain.Models.Others
{
    public class LogRequest
    {
        public Guid Id { get; private set; }
        public string Route { get; private set; }
        public string Controller { get; private set; }
        public string HttpVerb { get; private set; }
        public string QueryString { get; private set; }
        public string RequestBody { get; private set; }
        public string UserId { get; private set; }
        public string UserIp { get; private set; }
        public int HttpResponseStatusCode { get; private set; }
        public long ExecutionTime { get; private set; }
        public DateTime Date { get; private set; }

        public LogRequest(string route, string controller, string httpVerb, string queryString, string requestBody, string userId, string userIp, int httpResponseStatusCode, long executionTime)
        {
            Id = Guid.NewGuid();
            Route = route;
            Controller = controller;
            HttpVerb = httpVerb;
            QueryString = queryString;
            RequestBody = requestBody;
            UserId = userId;
            UserIp = userIp;
            HttpResponseStatusCode = httpResponseStatusCode;
            ExecutionTime = executionTime;
            Date = DateTime.Now;
        }
    }
}
