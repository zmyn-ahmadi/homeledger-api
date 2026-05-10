namespace HomeLedger.API.Services.Correlation;

// Helper service to consistently read the CorrelationId for the current HTTP request.
public interface ICorrelationIdAccessor
{
  // returns the current CorrelationId if available, or null if there is no active HttpContext.
  // Falls back to HttpContext.TraceIdentifier when a custom CorrelationId is not present.
  string? GetCorrelationId();
}
