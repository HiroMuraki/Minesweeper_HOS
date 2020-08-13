using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinesweepGameLite.MainCodes {
    public static class GeneralAction {
        
        public static void CopyInsertedFileToPath(string resourceName, string targetPath) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            Stream insertedResource = assembly.GetManifestResourceStream($@"{assemblyName}.{resourceName}");
            BufferedStream fileReader = new BufferedStream(insertedResource);
            FileStream fileWritter = File.OpenWrite($"{targetPath}");

            byte[] buffer = new byte[512];
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
