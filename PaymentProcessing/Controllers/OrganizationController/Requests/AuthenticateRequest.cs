namespace PaymentProcessing;

public record AuthenticateRequest(string OrgId, string OrgSecretKey)
{
}