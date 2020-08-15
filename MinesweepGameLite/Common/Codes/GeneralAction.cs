using System.IO;
using System.Reflection;

namespace Common {
    public static class GeneralAction {
        /// <summary>
        /// 用于将嵌入资源复制到指定目录
        /// </summary>
        /// <param name="resourceName">传递嵌入资源路径</param>
        /// <param name="targetPath">传递复制目标路径路径</param>
        public static void CopyInsertedFileToPath(string resourceName, string targetPath) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            Stream insertedResource = assembly.GetManifestResourceStream($@"{assemblyName}.{resourceName}");
            BufferedStream fileReader = new BufferedStream(insertedResource);
            FileStream fileWritter = File.OpenWrite($"{targetPath}");

            byte[] buffer = new byte[128];
            int length;
            do {
                length = fileReader.Read(buffer, 0, buffer.Length);
                fileWritter.Write(buffer, 0, length);
            } while (length > 0);

            insertedResource.Close();
            fileReader.Close();
            fileWritter.Close();
        }
    }
}
