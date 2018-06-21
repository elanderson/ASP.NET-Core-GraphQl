namespace ASP.NET_Core_GraphQl.Models
{
    using System;

    public class Droid : Character
    {
        public DateTime Created { get; set; }

        public string PrimaryFunction { get; set; }
    }
}
