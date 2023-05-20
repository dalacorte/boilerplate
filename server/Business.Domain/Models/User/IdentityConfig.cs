namespace Business.Domain.Model
{
    public class IdentityConfig
    {
        public string Secret { get; set; }
        public int Expires { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
    }
}
