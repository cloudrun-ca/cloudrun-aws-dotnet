using System;
using System.IO;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using CloudRun.Common.Configuation;

namespace CloudRun.AWS.Util
{
    public static class CredentialsHelper
    {
        static readonly AWSCredentials _creds = null;
        static readonly RegionEndpoint _defaultRegion = null;

        static CredentialsHelper()
        {
            if (AppEnvironment.IsDevelopment())
            {
                _creds = GetCredsForLocalDevelopment(out _defaultRegion);
            }
            else
            {
                _creds = new InstanceProfileAWSCredentials();
            }
        }

        public static AWSCredentials Credentials => _creds;
        public static RegionEndpoint DefaultRegion => _defaultRegion;

        static AWSCredentials GetCredsForLocalDevelopment(out RegionEndpoint region)
        {
            region = null;

            var profileName = Environment.GetEnvironmentVariable("AWS_PROFILE");

            if (string.IsNullOrEmpty(profileName))
                profileName = "default";

            var credsFileLoc = Environment.GetEnvironmentVariable("AWS_SHARED_CREDENTIALS_FILE");

            if (string.IsNullOrEmpty(credsFileLoc))
            {
                // this will not work for debugging under IIS
                var usersFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                credsFileLoc = Path.Combine(usersFolder, ".aws", "credentials");
            }

            SharedCredentialsFile sharedFile;

            try
            {
                sharedFile = new SharedCredentialsFile(credsFileLoc);
            }
            catch (Exception ex)
            {
                throw new Exception($"make sure {credsFileLoc} exists and your IIS app pool has read access to the file (if running under IIS / IIS Express)", ex);

                // find out the app pool under which the web app is running, find the file, add read permission to it
                // for example, if the app is running under DefaultAppPool, add this permission: IIS AppPool\DefaultAppPool
            }

            if (!sharedFile.TryGetProfile(profileName, out CredentialProfile credsProfile))
            {
                throw new Exception($"could not load AWS profile {profileName}");
            }

            ICredentialProfileSource sourceProfile = null;

            if (!string.IsNullOrEmpty(credsProfile.Options.SourceProfile))
            {
                sourceProfile = sharedFile;
            }

            var creds = credsProfile.GetAWSCredentials(sourceProfile);

            region = credsProfile.Region;

            return creds;
        }
    }
}