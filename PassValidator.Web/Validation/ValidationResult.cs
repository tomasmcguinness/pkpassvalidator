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
    }
}