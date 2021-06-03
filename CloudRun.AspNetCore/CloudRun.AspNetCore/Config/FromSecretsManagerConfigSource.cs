using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace CloudRun.AspNetCore.Config
{
    public class FromSecretsManagerConfigSource : JsonStreamConfigurationSource
    {
        public FromSecretsManagerConfigSource()
        {
        }

        public bool Optional { get; set; }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            // set the Stream property. This will subsequently be used when instantiating JsonStreamConfigurationProvider

            LoadStream();

            return base.Build(builder);
        }

        void LoadStream()
        {
            var key = $"{_serviceInfo.ServiceId.ToLowerInvariant()}/appSettings";

            try
            {
                var json = SecretsManager.Instance.WhisperAsync(key).GetAwaiter().GetResult();

                var ms = new MemoryStream();

                using var sw = new StreamWriter(ms, leaveOpen: true);

                sw.Write(json);

                sw.Flush();
                ms.Position = 0;

                Stream = ms;

                logger.Info("loaded appSettings from secrets manager");
            }
            catch (Exception ex) when (Optional)
            {
                logger.Warn(ex, "could not load appSettings from secrets manager");
            }
        }
    }
}