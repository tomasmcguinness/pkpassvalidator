namespace PassValidator.Web.Validation
{
    public class ValidationResult
    {
        public bool HasPass { get; set; }

        public bool HasManifest { get; set; }

        public bool HasSignature { get; set; }

        public bool TeamIdentifierMatches { get; set; }

        public bool PassTypeIdentifierMatches { get; set; }

        public bool SignedByApple { get; set; }

        public bool HasSignatureExpired { get; set; }

        public string SignatureExpirationDate { get; set; }

        public bool Has3xIcon { get; set; }

        public bool Has2xIcon { get; set; }

        public bool HasIcon1x { get; set; }

        public bool HasPassTypeIdentifier { get; set; }

        public bool HasTeamIdentifier { get; set; }

        public bool HasDescription { get; set; }

        public bool HasFormatVersion { get; set; }

        public bool HasSerialNumber { get; set; }

        public bool HasOrganizationName { get; set; }

        public bool HasAppLaunchUrl { get; set; }

        public bool HasAssociatedStoreIdentifiers { get; set; }
        public bool WWDRCertificateExpired { get; internal set; }
        public bool WWDRCertificateSubjectMatches { get; internal set; }
    }
}