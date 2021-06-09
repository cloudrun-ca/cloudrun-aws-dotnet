using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using CloudRun.Common.Security;

namespace CloudRun.AWS.Security
{
    // https://aws.amazon.com/blogs/security/how-to-use-aws-secrets-manager-client-side-caching-in-dotnet/
    // consider using the above, once compatibility issues with .NET Core 2.1 and 3.1 have been addressed
    // https://github.com/aws/aws-secretsmanager-caching-net/issues/9
    public class SecretsManager : ISecretsContainer, IDisposable
    {
        readonly AmazonSecretsManagerClient _client = null;

        private SecretsManager()
        {
            _client = SecretsManagerClientFactory.CreateClient();
        }

        public static SecretsManager Instance { get; } = new SecretsManager();

        public async Task<string> WhisperAsync(string secretId)
        {
            try
            {
                var req = new GetSecretValueRequest()
                {
                    SecretId = secretId
                };

                var result = await _client.GetSecretValueAsync(req);

                if (!string.IsNullOrEmpty(result.SecretString))
                {
                    return result.SecretString;
                }
                else
                {
                    using var sr = new StreamReader(result.SecretBinary);

                    var content = sr.ReadToEnd();

                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(content));

                    return decoded;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"could not whisper secret {secretId} from AWS Secrets Manager", ex);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _client?.Dispose();
            }
        }
    }
}
