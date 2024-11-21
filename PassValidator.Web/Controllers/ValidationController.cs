using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PassValidator.Validator;

namespace PassValidator.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValidationController : ControllerBase
{
    public async Task<IActionResult> Post(IFormFile file)
    {
        using var ms = new MemoryStream();
        file.CopyTo(ms);
        ms.Position = 0;

        var validator = new Validator.Validator();
        var result = validator.Validate(ms.ToArray());

        await LogResultAsync(result);

        return Ok(result);
    }

    private async Task LogResultAsync(ValidationResult result)
    {
        var connectionString = Environment.GetEnvironmentVariable("APPSETTING_table_storage");

        if (connectionString == null)
        {
            return;
        }

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
        CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
        CloudTable table = tableClient.GetTableReference("validations");

        var entity = new Validation(DateTime.UtcNow);

        entity.MissingFiles = !result.HasSignature || !result.HasManifest || !result.HasPass || !result.HasIcon1x;

        entity.MissingStandardKeys = !result.HasDescription ||
                                     !result.HasFormatVersion ||
                                     !result.HasOrganizationName ||
                                     !result.HasPassTypeIdentifier ||
                                     !result.HasSerialNumber ||
                                     !result.HasTeamIdentifier;

        entity.InvalidSignature = result.HasSignatureExpired || !result.SignedByApple || result.WwdrCertificateExpired || !result.WwdrCertificateSubjectMatches || !result.PassKitCertificateNameCorrect;

        entity.MissingIconFile = !result.HasIcon1x || !result.HasIcon2x || !result.HasIcon3x;

        entity.InvalidUpdateKeys = (result.HasWebServiceUrl || result.HasAuthenticationToken) && !result.WebServiceUrlIsHttps && !result.AuthenticationTokenRequiresWebServiceUrl || !result.WebServiceUrlRequiresAuthenticationToken;

        TableOperation insert = TableOperation.Insert(entity);

        await table.ExecuteAsync(insert);
    }

    class Validation : TableEntity
    {
        public Validation(DateTime date)
        {
            Date = date;
            PartitionKey = "PkpassValidator";
            RowKey = Guid.NewGuid().ToString();
        }

        public Validation()
        {

        }

        public DateTime Date { get; set; }

        public bool InvalidSignature { get; set; }

        public bool MissingFiles { get; set; }

        public bool MissingStandardKeys { get; set; }

        public bool MissingIconFile { get; set; }

        public bool InvalidUpdateKeys { get; set; }
    }
}