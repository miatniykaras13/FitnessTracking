namespace FitnessTracking.Api.Constants;

public static class ApiConstants
{
    public static class Security
    {
        public const string BearerScheme = "Bearer";
        public const string AuthorizationHeader = "Authorization";
        public const string JwtFormat = "JWT";
    }

    public static class Swagger
    {
        public const string JsonEndpoint = "/swagger/v1/swagger.json";
        public const string ApiVersionName = "FitnessTracking.Api v1";
        public const string DocsRoutePrefix = "docs";
    }

    public static class ProblemDetails
    {
        public const string ContentType = "application/problem+json";
        public const string TraceIdExtensionKey = "traceId";
        public const string ErrorCodeExtensionKey = "errorCode";
        public const string OtherProblemsExtensionKey = "otherProblems";
        public const string MultipleErrorsDetail = "Multiple errors occurred. See the 'otherProblems' extension for details.";
    }
}

