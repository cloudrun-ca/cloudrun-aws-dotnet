using System;
using CloudRun.Common.Util;

namespace CloudRun.Common.Configuation
{
    public static class AppEnvironment
    {
        static string _name = null;

        public static void SetName(string name)
        {
            _name = name;
        }

        public static bool IsDevelopment()
        {
            return Is(Development, "dev");
        }

        public static bool IsQA()
        {
            return Is(QA);
        }

        public static bool IsProduction()
        {
            return Is(Production, "prod");
        }

        public static bool Is(string name)
        {
            if (string.IsNullOrEmpty(_name))
            {
                _name = TryGetName();
            }

            return _name.Eq(name);
        }

        public static bool Is(params string[] names)
        {
            if (string.IsNullOrEmpty(_name))
            {
                _name = TryGetName();
            }

            return _name.IsEither(names);
        }

        public static readonly string Development = new Inferred();
        public static readonly string QA = new Inferred();
        public static readonly string Production = new Inferred();


        public static string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                    throw new Exception("Application environment is not set");

                return _name;
            }
        }

        // get name w/o throwing execption
        public static string TryGetName()
        {
            var result = _name;

            if (string.IsNullOrEmpty(_name))
                result = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            return result;
        }

        public static bool IsLambda()
        {
            var execEnv = Environment.GetEnvironmentVariable("AWS_EXECUTION_ENV");

            if (string.IsNullOrEmpty(execEnv))
                return false;

            // as per https://docs.aws.amazon.com/lambda/latest/dg/lambda-environment-variables.html

            var result = execEnv.StartsWith("AWS_Lambda_", StringComparison.OrdinalIgnoreCase);

            return result;
        }

        public static string LambdaVersion()
        {
            return Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_VERSION");
        }
    }
}
