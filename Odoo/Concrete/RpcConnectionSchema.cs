namespace Odoo.Concrete
{
    public class RpcConnectionSchema
    {
        private const string SuffixHost = "xmlrpc";
        private const string Common = "common";
        private const string Object = "object";
        private const string Report = "report";
        private int _UserId = -1;

        public RpcConnectionSchema(string serverUrl, string dbName, string dbUser, string dbPassword)
        {
            ServerUrl = serverUrl;
            DbName = dbName;
            DbUser = dbUser;
            DbPassword = dbPassword;
        }

        public string ServerUrl { get; }
        public string DbName { get; }
        public string DbUser { get; }
        public string DbPassword { get; }
        public int UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                _UserId = value;
            }
        }

        public string CommonUrl => $"{ServerUrl}/{SuffixHost}/{Common}";
        public string ObjectUrl => $"{ServerUrl}/{SuffixHost}/{Object}";
        public string ReportUrl => $"{ServerUrl}/{SuffixHost}/{Report}";
    }
}
