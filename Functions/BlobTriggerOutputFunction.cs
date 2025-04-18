using System.Text.Encodings;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Functions
{
    public class BlobTriggerOutputFunction
    {
        private readonly ILogger<BlobTriggerOutputFunction> _logger;

        public BlobTriggerOutputFunction(ILogger<BlobTriggerOutputFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(BlobTriggerOutputFunction))]
        [BlobOutput("sample-output/{name}-processed.txt")]
        public async Task<string> Run(
            [BlobTrigger("samples-workitems/{name}", Connection = "")] Stream stream,
            [BlobInput("samples-workitems/ko.txt")] Stream unused, // This line is necessary for the function to work as intended. ko.txt must be an existing file in the container.
            string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");

            return "hello" + content;
        }
    }
}
