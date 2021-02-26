// unset

namespace Authentication.API.Options
{
    public class PosApiClientOptions
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Secret { get; set; }
        public int TokenLifeTimeMinutes { get; set; }
    }
}