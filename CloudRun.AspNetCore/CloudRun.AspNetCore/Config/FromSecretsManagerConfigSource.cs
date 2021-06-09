using System;
using System.IO;
using CloudRun.AWS.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace CloudRun.AspNetCore.Config
{
    public class FromSecretsManagerConfigSource : JsonStreamConfigurationSource
    {
        readonly FromSecretsManagerConfigSourceOptions _options;

        public FromSecretsManagerConfigSource(Action<FromSecretsManagerConfigSourceOptions> setOptions)
        {
            var defaultOptions = new FromSecretsManagerConfigSourceOptions();

            setOptions?.Invoke(defaultOptions);

            if(string.IsNullOrEmpty(defaultOptions.SecretName))
            {
                throw new ArgumentNullException(nameof(FromSecretsManagerConfigSourceOptions.SecretName));
            }

            _options = defaultOptions;
        }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            // set the Stream property. This will subsequently be used when instantiating JsonStreamConfigurationProvider

            LoadStream();

            return base.Build(builder);
        }

        void LoadStream()
        {
            try
            {
                var json = SecretsManager.Instance.WhisperAsync(_options.SecretName).GetAwaiter().GetResult();

                var ms = new MemoryStream();

                using var sw = new StreamWriter(ms, leaveOpen: true);

                sw.Write(json);

                sw.Flush();
                ms.Position = 0;

                Stream = ms;
            }
            catch (Exception) when (_options.Optional)
            {
                // do nothing
            }
        }
    }

    public class FromSecretsManagerConfigSourceOptions
    {
        public bool Optional { get; set; }
        public string SecretName { get; set; }
    }
}