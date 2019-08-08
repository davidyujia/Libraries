using System;
using System.IO;
using System.Reflection;

namespace davidyujia.Unknow
{
    public static class EmbeddedResource
    {
        public static Stream Load(string nameSpace, string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream($"{nameSpace}.{fileName}");
            return stream;
        }
    }
}
