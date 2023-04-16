using LocationFindingService.Endpoints;
using System.Reflection;

namespace LocationFindingService.Extensions
{
    public static class HelperExtensions
    {
        public static void AddEndpointDefinitions(this IServiceCollection services)
        {
            var definitions = new List<IEndpointDefinition>();
            
            definitions.AddRange(Assembly.GetCallingAssembly().ExportedTypes
                .Where(x => typeof(IEndpointDefinition).IsAssignableFrom(x) && !x.IsInterface)
                .Select(Activator.CreateInstance).Cast<IEndpointDefinition>());

            foreach (var definition in definitions)
            {
                definition.DefineServices(services);
            }

            services.AddSingleton(definitions as IReadOnlyCollection<IEndpointDefinition>);
        }

        public static void UseEndpointDefinitions(this WebApplication app)
        {
            var definitions = app.Services.GetRequiredService<IReadOnlyCollection<IEndpointDefinition>>();

            foreach (var definition in definitions)
            {
                definition.DefineEndpoints(app);
            }
        }
    }
}
