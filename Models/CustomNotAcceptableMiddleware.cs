using System.Text.Json;

namespace HomeLedger.API.Models;

public class CustomNotAcceptableMiddleware(RequestDelegate next)
{
  private readonly RequestDelegate _next = next;

  // Invoke()
  public async Task Invoke(HttpContext context)
  {
    await _next(context);

    if (context.Response.StatusCode == StatusCodes.Status406NotAcceptable)
    {
      var acceptHeader = context.Request.Headers.Accept.ToString();

      context.Response.ContentType = "application/json";

      var response = new
      {
        Code = StatusCodes.Status406NotAcceptable,
        ErrorMessage = $"The Requested Format {acceptHeader} is Not Supported.",
      };
      await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
  }
}
