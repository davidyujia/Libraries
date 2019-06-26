using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace davidyujia.Configuration
{
    public static class Config
    {
        public static Func<string> GetEnvironmentFunc { private get; set; } = () => Environment;

        public static string Environment { get; set; }

        private static string GetEnv()
        {
            var env = Environment;

            if (string.IsNullOrWhiteSpace(env) && GetEnvironmentFunc != null)
            {
                env = GetEnvironmentFunc();
            }

            if (string.IsNullOrWhiteSpace(env))
            {
                env = System.Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            }

            if (string.IsNullOrWhiteSpace(env))
            {
                env = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            }

            return env;
        }

        private static IConfigurationBuilder GetBuilder(bool currentDirectory)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();

            if (currentDirectory)
            {
                builder = builder.SetBasePath(Directory.GetCurrentDirectory());
            }

            return builder; //.AddEnvironmentVariables();
        }

        private static IConfigurationBuilder GetFile(IConfigurationBuilder builder, string fileName)
        {
            var fileNameArray = fileName.Split('.').ToList();

            var ext = fileNameArray.Count > 1 ? "." + fileNameArray[fileNameArray.Count - 1] : string.Empty;

            if (ext.ToLower() != ".json")
            {
                throw new Exception("JSON FILE ONLY");
            }

            var fristName = fileNameArray.Count > 1 ? string.Join(".", fileNameArray.GetRange(0, fileNameArray.Count - 1)) : fileName;

            builder = builder.AddJsonFile(fileName, false, true);

            var env = GetEnv();

            if (!string.IsNullOrWhiteSpace(env))
            {
                builder = builder.AddJsonFile($"{fristName}.{env}{ext}", true, true);
            }

            return builder;
        }

        public static T Load<T>(bool currentDirectory = true) where T : new()
        {
            return Load<T>("appsettings.json", currentDirectory);
        }

        public static T Load<T>(string fileName, bool currentDirectory = true) where T : new()
        {
            object i = new T();

            var builder = GetBuilder(currentDirectory);

            builder = GetFile(builder, fileName);

            var config = builder.Build();

            config.Bind(i);

            return (T)i;
        }

        public static ConfigValue Load()
        {
            return Load("appsettings.json");
        }

        public static ConfigValue Load(string fileName, bool currentDirectory = true)
        {
            var builder = GetBuilder(currentDirectory);

            builder = GetFile(builder, fileName);

            var config = builder.Build();

            return new ConfigValue(config);
        }
    }

    public sealed class ConfigValue
    {
        IConfiguration _config;

        internal ConfigValue(IConfiguration config)
        {
            _config = config;
        }

        internal ConfigValue(IConfiguration config, string parent, string key)
        {
            _config = config;
            _parent = string.IsNullOrEmpty(parent) ? key : $"{parent}:{key}"; ;
        }

        private string _parent { get; set; }

        public override string ToString()
        {
            return _config[$"{_parent}"];
        }

        public IConfiguration Configuration()
        {
            return _config;
        }

        public ConfigValue this[string key]
        {
            get
            {
                return new ConfigValue(_config, _parent, key);
            }
        }
    }
}
