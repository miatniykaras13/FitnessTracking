namespace FitnessTracking.Shared.Constants;

public static class AuthConstants
{
    public const string SectionName = "Auth";
    public const string IssuerPath = "Auth:Issuer";
    public const string AudiencePath = "Auth:Audience";
    public const string SecretPath = "Auth:Secret";
    public const string IssuerKey = "Issuer";
    public const string AudienceKey = "Audience";
    public const string SecretKey = "Secret";
    public const string ExpiresInMinutesKey = "ExpiresInMinutes";
    public const double DefaultTokenLifetimeMinutes = 20d;
}

