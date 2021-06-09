using System;
using Amazon;

namespace CloudRun.AWS.Util
{
    public static class RegionConfig
    {
        static readonly RegionEndpoint _currentRegion = null;

        public static string DefaultRegionName = "us-east-1";

        static RegionConfig()
        {
            var regionName = Environment.GetEnvironmentVariable("AWS_REGION"); // this would be automatically available for Lambda via env vars

            if (string.IsNullOrEmpty(regionName))
            {
                try
                {
                    var ec2Region = Amazon.Util.EC2InstanceMetadata.Region;

                    // will be null for non-ec2 code
                    if (ec2Region != null)
                    {
                        _currentRegion = ec2Region;

                        return;
                    }
                }
                catch (Exception)
                {
                    // do nothing, we may not be running in EC2 env ...
                }
            }

            if (string.IsNullOrEmpty(regionName))
            {
                if(string.IsNullOrEmpty(DefaultRegionName))
                {
                    throw new ArgumentNullException(nameof(DefaultRegionName), "could not resolve region and default region is not set");
                }

                regionName = DefaultRegionName;
            }

            _currentRegion = RegionEndpoint.GetBySystemName(regionName);
        }

        public static RegionEndpoint CurrentRegion => _currentRegion;

    }
}