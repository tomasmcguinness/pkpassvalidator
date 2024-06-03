using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PassValidator.Web.Validation;

public class Validator
{
    private const string G4WwdrCertificateSerialNumber = "13DC77955271E53DC632E8CCFFE521F3CCC5CED2";

    public ValidationResult Validate(byte[] passContent)
    {
        var result = new ValidationResult();

        string manifestPassTypeIdentifier = null;
        string manifestTeamIdentifier = null;
        byte[] manifestFile = null;
        byte[] signatureFile = null;

        using var zipToOpen = new MemoryStream(passContent);
        using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read, false);
        foreach (var e in archive.Entries)
        {
            if (e.FullName.ToLower().Equals("manifest.json"))
            {
                result.HasManifest = true;

                using var stream = e.Open();
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                ms.Position = 0;
                manifestFile = ms.ToArray();
            }

            if (e.FullName.ToLower().Equals("pass.json"))
            {
                result.HasPass = true;

                using var stream = e.Open();
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                ms.Position = 0;
                var file = ms.ToArray();

                var jsonObject = JObject.Parse(Encoding.UTF8.GetString(file));

                manifestPassTypeIdentifier = GetKeyStringValue(jsonObject, "passTypeIdentifier");
                result.HasPassTypeIdentifier = !string.IsNullOrWhiteSpace(manifestPassTypeIdentifier);

                manifestTeamIdentifier = GetKeyStringValue(jsonObject, "teamIdentifier");
                result.HasTeamIdentifier = !string.IsNullOrWhiteSpace(manifestTeamIdentifier);

                var description = GetKeyStringValue(jsonObject, "description");
                result.HasDescription = !string.IsNullOrWhiteSpace(description);

                if (jsonObject.ContainsKey("formatVersion"))
                {
                    var formatVersion = jsonObject["formatVersion"].Value<int>();
                    result.HasFormatVersion = formatVersion == 1;
                }

                var serialNumber = GetKeyStringValue(jsonObject, "serialNumber");
                result.HasSerialNumber = !string.IsNullOrWhiteSpace(serialNumber);

                if (result.HasSerialNumber)
                {
                    result.HasSerialNumberOfCorrectLength = serialNumber.Length >= 16;
                }

                var organizationName = GetKeyStringValue(jsonObject, "organizationName");
                result.HasOrganizationName = !string.IsNullOrWhiteSpace(organizationName);

                if (jsonObject.ContainsKey("appLaunchURL"))
                {
                    result.HasAppLaunchUrl = true;
                    result.HasAssociatedStoreIdentifiers = jsonObject.ContainsKey("associatedStoreIdentifiers");
                }

                if (jsonObject.ContainsKey("webServiceURL"))
                {
                    result.HasWebServiceUrl = true;

                    var webServiceUrl = GetKeyStringValue(jsonObject, "webServiceURL");
                    result.WebServiceUrlIsHttps = webServiceUrl.ToLower().StartsWith("https://");
                }

                if (jsonObject.ContainsKey("authenticationToken"))
                {
                    result.HasAuthenticationToken = true;

                    var authToken = GetKeyStringValue(jsonObject, "authenticationToken");
                    result.AuthenticationTokenCorrectLength = authToken.Length >= 16;
                }

                if (result.HasAuthenticationToken && !result.HasWebServiceUrl)
                {
                    result.AuthenticationTokenRequiresWebServiceUrl = true;
                }

                if (result.HasWebServiceUrl && !result.HasAuthenticationToken)
                {
                    result.WebServiceUrlRequiresAuthenticationToken = true;
                }
            }

            if (e.FullName.ToLower().Equals("signature"))
            {
                result.HasSignature = true;

                using var stream = e.Open();
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                ms.Position = 0;
                signatureFile = ms.ToArray();
            }

            if (e.FullName.ToLower().Equals("icon.png"))
            {
                result.HasIcon1x = true;
            }

            if (e.FullName.ToLower().Equals("icon@2x.png"))
            {
                result.HasIcon2x = true;
            }

            if (e.FullName.ToLower().Equals("icon@3x.png"))
            {
                result.HasIcon3x = true;
            }
        }

        zipToOpen.Close();
        if (!result.HasManifest) return result;
            
        ContentInfo contentInfo = new ContentInfo(manifestFile);
        SignedCms signedCms = new SignedCms(contentInfo, true);

        signedCms.Decode(signatureFile);

        try
        {
            signedCms.CheckSignature(true);
        }
        catch
        {
            // ignored
        }

        var signer = signedCms.SignerInfos[0];

        // There are two certificates attached. One is the PassType certificate. One is the WWDR certificate.
        //
        X509Certificate2 passKitCertificate = null;

        foreach (var cert in signedCms.Certificates)
        {
            if (cert.SerialNumber == G4WwdrCertificateSerialNumber)
            {
                result.SignedByApple = true;
                result.WwdrCertificateFound = true;
                result.WwdrCertificateIsCorrectVersion = true;
            }
            else
            {
                // Find another cert issued by Apple Inc.
                var issuerName = new X509Name(cert.Issuer);

                var issuerCommonName = issuerName.GetValueList(X509Name.CN);
                var issuerOrganisation = issuerName.GetValueList(X509Name.O);

                if ((string)issuerOrganisation[0] == "Apple Inc." && (string)issuerCommonName[0] == "Apple Worldwide Developer Relations Certification Authority")
                {
                    passKitCertificate = cert;
                }
            }
        }

        if (passKitCertificate != null)
        {
            result.PassKitCertificateFound = true;

            foreach (var extension in passKitCertificate.Extensions)
            {
                // 1.2.840.113635.100.6.1.16 is the OID of the problematic part I think.
                // This is an Apple custom extension (1.2.840.113635.100.6.1.16) and in good passes, 
                // the value matches the pass type identifier.
                //
                if (extension.Oid.Value == "1.2.840.113635.100.6.1.16")
                {
                    var value = Encoding.ASCII.GetString(extension.RawData);
                    value = value.Substring(2, value.Length - 2);

                    result.PassKitCertificateNameCorrect = value == manifestPassTypeIdentifier;
                    break;
                }
            }

            result.PassKitCertificateExpired = passKitCertificate.NotAfter < DateTime.UtcNow;

            var issuerName = new X509Name(passKitCertificate.Issuer);
            var passKitIssuerOrg = issuerName.GetValueList(X509Name.O)[0] as string;
            var passKitIssuerCommonName = issuerName.GetValueList(X509Name.CN)[0] as string;

            var orgIsApple = passKitIssuerOrg == "Apple Inc.";
            var cnIsWwdr = passKitIssuerCommonName == "Apple Worldwide Developer Relations Certification Authority";

            result.PassKitCertificateIssuedByApple = orgIsApple && cnIsWwdr;
        }

        // Now check the subject and type identifier match.
        //
        var certName = new X509Name(signer.Certificate.Subject);

        var certificateCommonName = certName.GetValueList(X509Name.CN)[0] as string;
        var signaturePassTypeIdentifier = certificateCommonName.Replace("Pass Type ID: ", "");

        var certificateOrganisationUnit = certName.GetValueList(X509Name.OU)[0] as string;

        result.HasSignatureExpired = signer.Certificate.NotAfter < DateTime.UtcNow;
        result.SignatureExpirationDate = signer.Certificate.NotAfter.ToString("yyyy-MM-dd HH:mm:ss");

        result.PassTypeIdentifierMatches = manifestPassTypeIdentifier == signaturePassTypeIdentifier;
        result.TeamIdentifierMatches = manifestTeamIdentifier == certificateOrganisationUnit;

        return result;
    }

    private string GetKeyStringValue(JObject jsonObject, string key)
    {
        return jsonObject.ContainsKey(key) ? jsonObject[key].Value<string>() : null;
    }

    public static List<string> Parse(string data, string delimiter)
    {
        if (data == null) return null;
        if (!delimiter.EndsWith("=")) delimiter = delimiter + "=";
        if (!data.Contains(delimiter)) return null;
        //base case
        var result = new List<string>();
        var start = data.IndexOf(delimiter, StringComparison.Ordinal) + delimiter.Length;
        var length = data.IndexOf(',', start) - start;
        if (length == 0) return null; //the group is empty
        if (length > 0)
        {
            result.Add(data.Substring(start, length));
            //only need to recurse when the comma was found, because there could be more groups
            var rec = Parse(data.Substring(start + length), delimiter);
            if (rec != null) result.AddRange(rec); //can't pass null into AddRange() :(
        }
        else //no comma found after current group so just use the whole remaining string
        {
            result.Add(data.Substring(start));
        }
        return result;
    }
}