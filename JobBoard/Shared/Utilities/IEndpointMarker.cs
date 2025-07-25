namespace JobBoard.Shared.Utilities;

public interface IEndpointMarker
{
    RouteHandlerBuilder Register(IEndpointRouteBuilder app);

}
