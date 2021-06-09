using System;
using System.Threading.Tasks;

namespace CloudRun.Common.Security
{
    public interface ISecretsContainer
    {
        Task<string> WhisperAsync(string secretId);
    }
}
