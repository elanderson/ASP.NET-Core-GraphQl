namespace ASP.NET_Core_GraphQl.Options
{
    using System;

    public class EndpointOptions
    {
        public Uri Url { get; set; }

        public CertificateOptions Certificate { get; set; }
    }
}
