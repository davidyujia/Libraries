using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using davidyujia.Crypto;
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
            _thisKey = string.IsNullOrEmpty(parent) ? key : $"{parent}:{key}"; ;
        }

        private string _thisKey { get; set; }

        public override string ToString() => _config[$"{_thisKey}"];

        public ConnectionString ConnectionString => string.IsNullOrEmpty(_thisKey) ? null : new ConnectionString(ToString());

        public IConfiguration Configuration => _config;

        public ConfigValue this[string key] => new ConfigValue(_config, _thisKey, key);
    }

    public sealed class CrypterConfig
    {
        internal static Crypter Crypter = new Crypter();

        internal static string SecretString = "Secret:";

        public static string Encryptor(string source)
        {
            return SecretString + Crypter.Encrypt(source);
        }

        public static string Decrypt(string encryptString)
        {
            if (!IsNeedDecryptor(encryptString))
            {
                return encryptString;
            }

            var data = encryptString.Substring(CrypterConfig.SecretString.Length, encryptString.Length - CrypterConfig.SecretString.Length);

            return Crypter.Decrypt(data);
        }

        private static bool IsNeedDecryptor(string str) => string.IsNullOrWhiteSpace(str) ? false : str.StartsWith(CrypterConfig.SecretString);
    }

    public sealed class ConnectionString
    {
        private SecureString _value;

        internal ConnectionString(string value)
        {
            _value = new SecureString();

            foreach (var c in CrypterConfig.Decrypt(value))
            {
                _value.AppendChar(c);
            }
        }

        public override string ToString()
        {
            var ptr = Marshal.SecureStringToBSTR(_value);

            return Marshal.PtrToStringBSTR(ptr);
        }
    }
}
