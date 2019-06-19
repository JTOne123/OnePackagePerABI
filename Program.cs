using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

//https://docs.microsoft.com/en-us/xamarin/android/deploy-test/building-apps/build-process
namespace APKBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            var packageName = "[[your.package.name or new one]]";
            int versionCode = 10;
            var versionName = "1.2.3";

            var msbuild = @"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\msbuild.exe";
            //var msbuild = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin\msbuild.exe";
            var zipalign = @"C:\Android\sdk\build-tools\27.0.3\zipalign.exe";
            var jarsigner = @"C:\Program Files\Java\jdk1.8.0_161\bin\jarsigner.exe";

            var buildManifest = "Properties/AndroidManifest.xml";
            var androidProjectFolder = @"C:\Git\[[Path to project]].Droid";
            var androidProject = $"{androidProjectFolder}\\[[Project]].Mobile.Droid.csproj";
            var outputPath = @"C:\temp\" + DateTime.Now.ToString("yyyyMMddHHmmss");

            var abis = new string[] { "armeabi", "armeabi-v7a", "x86", "arm64-v8a", "x86_64" };
            for (int i = 0; i < abis.Length; i++)
            {
                var abi = abis[i];

                //I don't need this ABI. Do you?
                if (abi == "armeabi")
                    continue;

                var specificManifest = $"Properties/AndroidManifest.{abi}_{versionCode}.xml";
                var binPath = $"{outputPath}/{abi}/bin";
                var objPath = $"{outputPath}/{abi}/obj";

                var keystorePath = $"\"{androidProjectFolder}/[[KeyStore file name]].keystore\"";
                var keystorePassword = "[[KeyStore password]]";
                var keystoreKey = "[[KeyStore Key]]";

                File.Copy($"{androidProjectFolder}/{buildManifest}", $"{androidProjectFolder}/{specificManifest}", true);

                var xmlFile = XDocument.Load($"{androidProjectFolder}/{specificManifest}");
                var mnfst = xmlFile.Elements("manifest").First();
                var androidNamespace = mnfst.GetNamespaceOfPrefix("android");
                mnfst.Attribute("package").Value = packageName;
                mnfst.Attribute(androidNamespace + "versionName").Value = versionName;
                mnfst.Attribute(androidNamespace + "versionCode").Value = ((i + 1) * 100000 + versionCode).ToString();
                xmlFile.Save($"{androidProjectFolder}/{specificManifest}");

                var unsignedApkPath = $"\"{binPath}/{packageName}.apk\"";
                var signedApkPath = $"\"{binPath}/{packageName}_signed.apk\"";
                var alignedApkPath = $"{binPath}/{packageName}_signed_aligned.{abi}.apk";

                var mbuildArgs = $"{androidProject} /t:PackageForAndroid /p:AndroidSupportedAbis={abi} /p:AndroidManifest={specificManifest} /p:Configuration=Release /p:IntermediateOutputPath={objPath}/ /p:OutputPath={binPath}";
                var jarsignerArgs = $"-verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore {keystorePath} -storepass {keystorePassword} -signedjar \"{signedApkPath}\" {unsignedApkPath} {keystoreKey}";
                var zipalignArgs = $"-f -v 4 {signedApkPath} {alignedApkPath}";

                RunProcess(msbuild, mbuildArgs);
                Console.WriteLine("Build is done");

                RunProcess(jarsigner, jarsignerArgs);
                Console.WriteLine("Jarsigner is done");

                //This is should be the last step otherwise Google Play Store will not accept the APK
                RunProcess(zipalign, zipalignArgs);
                Console.WriteLine("Zipalign is done");

                File.Copy($"{alignedApkPath}", $"{outputPath}/{Path.GetFileName(alignedApkPath)}", true);
            }

            Console.WriteLine("Built and signed");
            Console.ReadKey();
        }

        static void RunProcess(string filename, string arguments)
        {
            var process = new Process();
            process.StartInfo.FileName = filename;
            process.StartInfo.Arguments = arguments;
            process.Start();
            process.WaitForExit();
        }
    }
}
