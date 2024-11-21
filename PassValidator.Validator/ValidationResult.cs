namespace PassValidator.Validator;

public class ValidationResult
{
    public bool HasPass { get; set; }

    public bool HasManifest { get; set; }

    public bool HasSignature { get; set; }

    public bool TeamIdentifierMatches { get; set; }

    public bool PassTypeIdentifierMatches { get; set; }

    public bool SignedByApple { get; set; }

    public bool HasSignatureExpired { get; set; } = true;

    public string SignatureExpirationDate { get; set; }

    public bool HasIcon3x { get; set; }

    public bool HasIcon2x { get; set; }

    public bool HasIcon1x { get; set; }

    public bool HasPassTypeIdentifier { get; set; }

    public bool HasTeamIdentifier { get; set; }

    public bool HasDescription { get; set; }

    public bool HasFormatVersion { get; set; } = false;

    public bool HasSerialNumber { get; set; }

    public bool HasSerialNumberOfCorrectLength { get; set; }
    public bool HasOrganizationName { get; set; }

    public bool HasAppLaunchUrl { get; set; }

    public bool HasAssociatedStoreIdentifiers { get; set; }

    public bool WwdrCertificateExpired { get; set; } = true;

    public bool WwdrCertificateSubjectMatches { get; set; }

    public bool WwdrCertificateIsCorrectVersion { get; set; }

    public bool HasAuthenticationToken { get; set; }

    public bool HasWebServiceUrl { get; set; }

    public bool WebServiceUrlIsHttps { get; set; }

    public bool AuthenticationTokenRequiresWebServiceUrl { get; set; }

    public bool WebServiceUrlRequiresAuthenticationToken { get; set; }

    public bool PassKitCertificateNameCorrect { get; set; }

    public bool PassKitCertificateExpired { get; set; } = true;

    public bool WwdrCertificateFound { get; set; }

    public bool PassKitCertificateFound { get; set; }

    public bool AuthenticationTokenCorrectLength { get; internal set; }
    public bool PassKitCertificateIssuedByApple { get; internal set; }
}