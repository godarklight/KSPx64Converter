Place next to KSP.exe and run KSPx64Converter.  
  
To build this project you need to follow these steps:  
  
1) Download unity 4.6.4  
2) Open UnitySetup.exe with 7zip  
3) Extract "$_OUTDIR\windowsstandalonesupport\Variations\win64_nondevelopment\player_win" and rename it KSP.exe  
4) Extract "$_OUTDIR\windowsstandalonesupport\Variations\win64_development\Data\Mono\mono.dll"  
5) Gzip compress both files - they should be named mono.dll.gz and KSP.exe.gz. Do *not* tarball them.  
6) Place both files in KSPx64Converter  
7) Build KSPx64Converter  
8) Enjoy!  
  
To port to a new version of KSP, first manually find the correct version of unity for KSP (KSP.log is a good indicator), then follow the above steps.  
Also change SUPPORTED_HASH to the sha256sum of KSP/KSP_Data/Mono/mono.dll  
