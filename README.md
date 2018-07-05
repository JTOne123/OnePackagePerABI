# This is a super easy script for building your android APK files with different versionCode, versionName, packadgeName and separated ABI
[There is no ROR, only clear C# code]

This is a simple C# console app to build one ABI per APK.

Inspired by this 
example https://github.com/xamarin/monodroid-samples/tree/master/OneABIPerAPK

Also, you might set up different versionCode, versionName, and packageName.

APKs are fully ready for Google Play.

# How to start
1. Create a simple console app at Visual Studio and copy the code from **Program.cs**
2. Replace text placeholders
3. Run (F5)
4. Go drink a coffee

*[[your.package.name or new one]]* - you can easily duplicate your app on Google Play

*[[Project]]* - android project file name (.csproj)

*[[Path to project]]* - path where the [[Project]] is located

*[[KeyStore file name]]* - path to your keystore file

*[[KeyStore password]]* - keystore password

*[[KeyStore Key]]* - keystore key
