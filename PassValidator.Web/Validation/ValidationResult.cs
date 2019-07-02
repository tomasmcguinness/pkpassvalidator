using System;

namespace PassValidator.Web.Validation
{
    public class ValidationResult
    {
        public ValidationResult()
        {
        }

        public bool IsValid { get; set; }

        public bool HasPass { get; internal set; }
        public bool HasManifest { get; internal set; }
        public bool HasSignature { get; internal set; }
        public bool TeamIdentifierMatches { get; internal set; }
        public bool PassTypeIdentifierMatches { get; internal set; }
        public bool SignedByApple { get; internal set; }
        public bool HasSignatureExpired { get; internal set; }
        public string SignatureExpirationDate { get; internal set; }
        public bool Has3xIcon { get; internal set; }
        public bool Has2xIcon { get; internal set; }
        public bool HasIcon1x { get; internal set; }
        public bool HasPassTypeIdentifier { get; internal set; }
        public bool HasTeamIdentifier { get; internal set; }
        public bool HasDescription { get; internal set; }
        public bool HasFormatVersion { get; internal set; }
        public bool HasSerialNumber { get; internal set; }
        public bool HasOrganizationName { get; internal set; }
    }
}