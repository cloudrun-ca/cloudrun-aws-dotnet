using System;
using Amazon.SecretsManager;
using CloudRun.AWS.Util;
using CloudRun.Common.Configuation;

namespace CloudRun.AWS.Security
{
    public static class SecretsManagerClientFactory
    {
        public static AmazonSecretsManagerClient CreateClient()
        {
            return CreateClient(AppEnvironment.IsLambda());
        }

        public static AmazonSecretsManagerClient CreateClient(bool isLambda)
        {
            AmazonSecretsManagerClient client;

            if (isLambda && !AppEnvironment.IsDevelopment())
            {
                client = new AmazonSecretsManagerClient();
            }
            else
            {
                client = new AmazonSecretsManagerClient(CredentialsHelper.Credentials, RegionConfig.CurrentRegion);
            }

            return client;
        }
    }
}
