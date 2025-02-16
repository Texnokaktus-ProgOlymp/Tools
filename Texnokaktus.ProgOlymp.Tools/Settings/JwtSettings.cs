namespace Texnokaktus.ProgOlymp.Tools.Settings;

public class JwtSettings
{
    public string Issuer { get; init; }
    public string Audience { get; init; }
    public string IssuerSigningKey { get; init; }
    public TimeSpan Validity { get; init; }
}
