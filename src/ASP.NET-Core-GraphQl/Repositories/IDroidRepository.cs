namespace ASP.NET_Core_GraphQl.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ASP.NET_Core_GraphQl.Models;

    public interface IDroidRepository
    {
        Task<Droid> GetDroid(Guid id, CancellationToken cancellationToken);

        Task<List<Character>> GetFriends(Droid droid, CancellationToken cancellationToken);
    }
}
