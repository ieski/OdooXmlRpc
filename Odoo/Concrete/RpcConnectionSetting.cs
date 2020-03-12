namespace Odoo.Concrete
{
    public class RpcConnectionSetting
    {
        public string ServerUrl { get; set; }
        public string DbName { get; set; }
        public string DbUser { get; set; }
        public string DbPassword { get; set; }
        public bool ImmediateLogin { get; set; }
        public bool ServerCertificateValidation { get; set; }
    }
}
