using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography.Pkcs;
using System.Text;

namespace PassValidator.Web.Validation
{
    public class Validator
    {
        public ValidationResult Validate(byte[] passContent)
        {
            ValidationResult result = new ValidationResult();

            string passTypeIdentifier = null;
            string teamIdentifier = null;
            string signaturePassTypeIdentifier = null;
            string signatureTeamIdentifier = null;
            string expirationDate = null;
            byte[] manifestFile = null;
            byte[] signatureFile = null;

            using (MemoryStream zipToOpen = new MemoryStream(passContent))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read, false))
                {
                    foreach (var e in archive.Entries)
                    {
                        if (e.Name.ToLower().Equals("manifest.json"))
                        {
                            result.HasManifest = true;

                            using (var stream = e.Open())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    stream.CopyTo(ms);
                                    ms.Position = 0;
                                    manifestFile = ms.ToArray();
                                }
                            }
                        }

                        if (e.Name.ToLower().Equals("pass.json"))
                        {
                            result.HasPass = true;

                            using (var stream = e.Open())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    stream.CopyTo(ms);
                                    ms.Position = 0;
                                    var file = ms.ToArray();

                                    var jsonObject = JObject.Parse(Encoding.UTF8.GetString(file));

                                    passTypeIdentifier = jsonObject["passTypeIdentifier"].Value<string>();
                                    result.HasPassTypeIdentifier = !string.IsNullOrWhiteSpace(passTypeIdentifier);

                                    teamIdentifier = jsonObject["teamIdentifier"].Value<string>();
                                    result.HasTeamIdentifier = !string.IsNullOrWhiteSpace(teamIdentifier);

                                    var description = jsonObject["description"].Value<string>();
                                    result.HasDescription = !string.IsNullOrWhiteSpace(description);

                                    var formatVersion = jsonObject["formatVersion"].Value<int>();
                                    result.HasFormatVersion = formatVersion == 1;

                                    var serialNumber = jsonObject["serialNumber"].Value<string>();
                                    result.HasSerialNumber = !string.IsNullOrWhiteSpace(serialNumber);

                                    var organizationName = jsonObject["organizationName"].Value<string>();
                                    result.HasOrganizationName = !string.IsNullOrWhiteSpace(organizationName);

                                    if (jsonObject.ContainsKey("expirationDate"))
                                    {
                                        expirationDate = jsonObject["expirationDate"].Value<string>();
                                    }
                                }
                            }
                        }

                        if (e.Name.ToLower().Equals("signature"))
                        {
                            result.HasSignature = true;

                            using (var stream = e.Open())
                            {
                                using (var ms = new MemoryStream())
                                {
                                    stream.CopyTo(ms);
                                    ms.Position = 0;
                                    signatureFile = ms.ToArray();
                                }

                            }
                        }

                        if (e.Name.ToLower().Equals("icon.png"))
                        {
                            result.HasIcon1x = true;
                        }

                        if (e.Name.ToLower().Equals("icon@2x.png"))
                        {
                            result.Has2xIcon = true;
                        }

                        if (e.Name.ToLower().Equals("icon@3x.png"))
                        {
                            result.Has3xIcon = true;
                        }
                    }
                }
            }

            ContentInfo contentInfo = new ContentInfo(manifestFile);
            SignedCms signedCms = new SignedCms(contentInfo, true);

            signedCms.Decode(signatureFile);

            try
            {
                signedCms.CheckSignature(true);
            }
            catch
            {

            }

            var signer = signedCms.SignerInfos[0];

            result.SignedByApple = signer.Certificate.IssuerName.Name == "CN=Apple Worldwide Developer Relations Certification Authority, OU=Apple Worldwide Developer Relations, O=Apple Inc., C=US";


            if (result.SignedByApple)
            {
                Debug.WriteLine(signer.Certificate);

                var cnValues = Parse(signer.Certificate.Subject, "CN");
                var ouValues = Parse(signer.Certificate.Subject, "OU");

                var passTypeIdentifierSubject = cnValues[0];
                signaturePassTypeIdentifier = passTypeIdentifierSubject.Replace("Pass Type ID: ", "");

                if (ouValues != null && ouValues.Count > 0)
                {
                    signatureTeamIdentifier = ouValues[0];
                }

                Debug.WriteLine(signer.Certificate.IssuerName.Name);

                result.HasSignatureExpired = signer.Certificate.NotAfter < DateTime.UtcNow;
                result.SignatureExpirationDate = signer.Certificate.NotAfter.ToString("yyyy-MM-dd HH:mm:ss");
            }

            result.PassTypeIdentifierMatches = passTypeIdentifier == signaturePassTypeIdentifier;
            result.TeamIdentifierMatches = teamIdentifier == signatureTeamIdentifier;

            return result;
        }

        public static List<string> Parse(string data, string delimiter)
        {
            if (data == null) return null;
            if (!delimiter.EndsWith("=")) delimiter = delimiter + "=";
            if (!data.Contains(delimiter)) return null;
            //base case
            var result = new List<string>();
            int start = data.IndexOf(delimiter) + delimiter.Length;
            int length = data.IndexOf(',', start) - start;
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
}
