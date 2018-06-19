namespace ASP.NET_Core_GraphQl.Schemas
{
    using GraphQL;
    using GraphQL.Types;

    public class MainSchema : Schema
    {
        public MainSchema(
            RootQuery query,
            RootMutation mutation,
            RootSubscription subscription,
            IDependencyResolver resolver)

            : base(resolver)
        {
            this.Query = resolver.Resolve<RootQuery>();
            this.Mutation = mutation;
            this.Subscription = subscription;
        }
    }
}
