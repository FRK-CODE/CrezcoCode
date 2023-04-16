namespace LocationFindingService.Endpoints
{
    /// <summary>
    /// Defines the methods required for registering the endpoints as well as required DI services
    /// </summary>
    public interface IEndpointDefinition
    {
        /// <summary>
        /// Defines the minimal API endpoints
        /// </summary>
        /// <param name="app"></param>
        public void DefineEndpoints(WebApplication app);

        /// <summary>
        /// Defines the required DI services for the end points
        /// </summary>
        /// <param name="services"></param>
        public void DefineServices(IServiceCollection services);
    }
}
