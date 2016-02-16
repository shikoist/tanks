REM Copy directory MapPacks.
cd Windows
md MapPacks
cd ..
cd Windows_x64
md MapPacks
cd ..
cd Linux
md MapPacks
cd ..
cd MacOS
md MapPacks
cd ..
cd ..
copy MapPacks\*.* Builds\Windows\MapPacks
copy MapPacks\*.* Builds\Windows_x64\MapPacks
copy MapPacks\*.* Builds\Linux\MapPacks
copy MapPacks\*.* Builds\MacOS\MapPacks
cd Builds
7za a -tzip Tanks_Windows_x86.zip Windows\
7za a -tzip Tanks_Windows_x64.zip Windows_x64\
7za a -tzip Tanks_Linux.zip Linux\
7za a -tzip Tanks_MacOS.zip MacOS\
pause