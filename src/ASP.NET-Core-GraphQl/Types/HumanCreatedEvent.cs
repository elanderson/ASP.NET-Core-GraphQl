namespace ASP.NET_Core_GraphQl.Types
{
    using ASP.NET_Core_GraphQl.Repositories;

    public class HumanCreatedEvent : HumanObject
    {
        public HumanCreatedEvent(IHumanRepository humanRepository) : base(humanRepository)
        {
        }
    }
}
