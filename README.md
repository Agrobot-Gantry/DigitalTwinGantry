# DigitalTwinGantry

This application is meant for VR with the Oculus Quest 2.
It can also be built as a desktop application.

It is made with the unity version: 2020.3.32f1 with the android build support.

## Building VR version
1. Set in unity hub the target platform to android.
2. Start the project in unity.
3. Go to file -  build settings.
4. Press 'build' or 'build and run' and choose the directory you want the files in.

## Uploading VR apk to the Oculus Quest 2
1. [Install ADB.](https://www.xda-developers.com/install-adb-windows-macos-linux/)
2. Open command prompt and enter: `adb install <apk-path>`.

replace `<apk-path>` with the file path leading to DigitalTwinGantry.apk.

## Building desktop version
1. Set in unity hub the target platform to current platform.
2. Start the project in unity.
3. Go to file - build settings.
4. Press 'build' or 'build and run' and choose the directory you want the files in.




## If you already started the project in unity, you can change the version like this:
1. Go to file - build settings
2. Select PC, Mac & Linux standalone and press 'switch platform' if you want the desktop version.
3. Select Android and press 'switch platform' if you want the VR version.
4. Press 'build or 'build and run' and choose the direrectory you want the files in
