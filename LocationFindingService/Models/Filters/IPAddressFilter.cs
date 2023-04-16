using System.Net;

namespace LocationFindingService.Models.Filters
{
    public class IPAddressFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            string? argToValidate = context.GetArgument<string>(0);

            bool isIPAddress = IPAddress.TryParse(argToValidate, out _);

            if (!isIPAddress)
            {
                return Results.ValidationProblem(
                    new Dictionary<string, string[]>()
                    {
                        { "Invalid IP Address", new[]{ "A valid IP Address must be provided" } }
                    },
                    statusCode: (int)HttpStatusCode.BadRequest);
            }

            return await next.Invoke(context);
        }
    }
}
