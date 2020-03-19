namespace PassValidator.Lib.Validation
{
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

        public bool HasFormatVersion { get; set; }

        public bool HasSerialNumber { get; set; }

        public bool hasSerialNumberOfCorrectLength { get; set; }
        public bool HasOrganizationName { get; set; }

        public bool HasAppLaunchUrl { get; set; }

        public bool HasAssociatedStoreIdentifiers { get; set; }

        public bool WWDRCertificateExpired { get; set; } = true;

        public bool WWDRCertificateSubjectMatches { get; set; }

        public bool HasAuthenticationToken { get; set; }

        public bool AuthenticationTokenIsCorrectLength { get; set; }

        public bool HasWebServiceUrl { get; set; }

        public bool WebServiceUrlIsHttps { get; set; }
    }
}