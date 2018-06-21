namespace ASP.NET_Core_GraphQl
{
    using System;
    using Boxed.AspNetCore;
    using CorrelationId;
    using ASP.NET_Core_GraphQl.Constants;
    using GraphQL.Server.Transports.AspNetCore;
    using GraphQL.Server.Transports.WebSockets;
    using GraphQL.Server.Ui.Playground;
    using ASP.NET_Core_GraphQl.Schemas;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The main start-up class for the application.
    /// </summary>
    public class Startup : IStartup
    {
        private readonly IConfiguration configuration;
        private readonly IHostingEnvironment hostingEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration, where key value pair settings are stored. See
        /// http://docs.asp.net/en/latest/fundamentals/configuration.html</param>
        /// <param name="hostingEnvironment">The environment the application is running under. This can be Development,
        /// Staging or Production by default. See http://docs.asp.net/en/latest/fundamentals/environments.html</param>
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            this.configuration = configuration;
            this.hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Configures the services to add to the ASP.NET Core Injection of Control (IoC) container. This method gets
        /// called by the ASP.NET runtime. See
        /// http://blogs.msdn.com/b/webdev/archive/2014/06/17/dependency-injection-in-asp-net-vnext.aspx
        /// </summary>
        public IServiceProvider ConfigureServices(IServiceCollection services) =>
            services
                .AddCorrelationIdFluent()
                .AddCustomCaching()
                .AddCustomOptions(this.configuration)
                .AddCustomRouting()
                .AddCustomResponseCompression()
                .AddHttpContextAccessor()
                // Add useful interface for accessing the ActionContext.
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                // Add useful interface for accessing the IUrlHelper.
                .AddScoped(x => x
                    .GetRequiredService<IUrlHelperFactory>()
                    .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext))
                .AddMvcCore()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                    .AddAuthorization()
                    .AddJsonFormatters()
                    .AddCustomJsonOptions(this.hostingEnvironment)
                    .AddCustomCors()
                    .AddCustomMvcOptions(this.hostingEnvironment)
                .Services
                .AddCustomGraphQL(this.hostingEnvironment)
                .AddGraphQLRelayTypes()
                .AddProjectRepositories()
                .AddProjectGraphQLTypes()
                .AddProjectGraphQLSchemas()
                .BuildServiceProvider();

        /// <summary>
        /// Configures the application and HTTP request pipeline. Configure is called after ConfigureServices is
        /// called by the ASP.NET runtime.
        /// </summary>
        public void Configure(IApplicationBuilder application) =>
            application
                // Pass a GUID in a X-Correlation-ID HTTP header to set the HttpContext.TraceIdentifier.
                .UseCorrelationId()
                .UseForwardedHeaders()
                .UseResponseCompression()
                // Add the GraphQL playground UI to try out the GraphQL API. Not recommended to be run in production.
                .UseIf(
                    this.hostingEnvironment.IsDevelopment(),
                    x => x.UseGraphQLPlayground(new GraphQLPlaygroundOptions() { Path = "/" }))
                .UseCors(CorsPolicyName.AllowAny)
                .UseIf(
                    this.hostingEnvironment.IsDevelopment(),
                    x => x.UseDeveloperErrorPages())
                .UseStaticFilesWithCacheControl()
                .UseWebSockets()
                .UseGraphQLWebSocket<MainSchema>(new GraphQLWebSocketsOptions())
                .UseGraphQLHttp<MainSchema>(
                    new GraphQLHttpOptions()
                    {
                        // Show stack traces in exceptions. Don't turn this on in production.
                        ExposeExceptions = this.hostingEnvironment.IsDevelopment(),
                        // Add your own validation rules e.g. for authentication.
                        // ValidationRules = new[] { new RequiresAuthValidationRule() }.Concat(DocumentValidator.CoreRules());
                    });
    }
}