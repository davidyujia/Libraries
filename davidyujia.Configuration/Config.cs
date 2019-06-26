using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace davidyujia.Configuration
{
    public static class Config
    {
        private static string _environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

        private static IConfigurationBuilder GetBuilder(bool currentDirectory)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder();

            if (currentDirectory)
            {
                builder = builder.SetBasePath(Directory.GetCurrentDirectory());
            }

            return builder;
        }

        private static IConfigurationBuilder GetJsonFile(IConfigurationBuilder builder, string fileName)
        {
            var fileNameArray = fileName.Split('.').ToList();
            var ext = fileNameArray.Count > 1 ? "." + fileNameArray[fileNameArray.Count - 1] : string.Empty;

            var fristName = fileNameArray.Count > 1 ? string.Join(".", fileNameArray.GetRange(0, fileNameArray.Count - 1)) : fileName;

            builder = builder
            .AddJsonFile(fileName, true, true)
            .AddJsonFile($"{fileName}.{_environment}{ext}", true, true);

            return builder;
        }

        public static T Load<T>(bool currentDirectory = true) where T : new()
        {
            return Load<T>("appsettings.json", currentDirectory);
        }

        public static T Load<T>(string fileName, bool currentDirectory = true) where T : new()
        {
            object i = new T();

            var _config = GetBuilder(currentDirectory)
                 .AddJsonFile($"{fileName}", true, true)
                 .AddJsonFile($"{fileName}.{_environment}", true, true)
                 .AddEnvironmentVariables()
                 .Build();

            _config.Bind(i);

            return (T)i;
        }

        public static IConfiguration Load()
        {
            return Load("appsettings.json");
        }

        public static IConfiguration Load(string fileName, bool currentDirectory = true)
        {
            return GetBuilder(currentDirectory)
                .AddJsonFile($"{fileName}.json", true, true)
                .AddJsonFile($"{fileName}.{_environment}.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
