# AppUpdater
### Introduction

I have created this console application program to help a friend. He wanted to easily update his 2D game without asking his testers to re-download the game. I have come up with this as a solution to his problem and also thought it would be a fun project for me to gain some C# programming experience.

### Compatibility
Windows only

### What is AppUpdate?
AppUpdate is an open source console application that allows you to update your software/program. You do not need programming experience to operate AppUpdate, but you need basic understanding of C# programming if you want to make changes to the console application.

### Features
  - Connects to your web server to check for new updates
  - When you release 3 versions in a day, your users receive all of the patches in the correct order
  - Opens your software/project exe file after update check is complete.
  - Logs errors

### How does AppUpdate work?
Within a software/program, there is an .exe file that starts the software/program. For the purposes of explaining how AppUpdate works, Let’s assume your start up program is called “start.exe”. Instead of asking your users to open start.exe you ask them to open AppUpdate.exe (you can rename AppUpdate.exe of course).

When AppUpdate runs, the application connects to a public web address to check a “version.txt” file. Based on this txt file, the console application compares to the local version. “start.exe” runs automatically after the update process.

### How to use AppUpdate?

The way it works is quite simple. To use it, you must download these two files from this repository:
  - AppUpdate.exe
  - AppUpdate Config Editor.exe

You need four things,  
  - A web server
  - Public domain
  - FTP access
  - Basic knowledge on how to use a web server & ftp.

## Setting this up

I assume that you have a public domain address which as an example can be traditionally written like this `example.com`, and you have access to the `public_html` directory using FTP.

I also assume you know how to use an FTP client. If not, Google is your friend.

### Step 1
  - Download and copy `AppUpdate.exe` to your project directory.
  - Download `AppUpdate Config Editor.exe` and save it to your desktop

The config editor will allow you to generate a `update.bin` file which will hold the information:
  - Link to your public update url
  - Name of the file that will be opened after a successful update
  - The local version number of the current update.

Open `AppUpdate Config Editor.exe` and fill in the fields. 

  - Link: http://your-site.com/update/
  - Version: 0
  - Launch: **start.exe**

*Save As*:  **update.bin**, make sure update.bin is placed in the same directory as **AppUpdate.exe**.

### Step 2

You need to connect to your web server using an **FTP client**. Navigate yourself to the `public_html` directory. Create a new directory and call it `update`.

The **update** directory is where all your releases/patches will be stored.

### Step 3

Within the update directory on your Web Server, create a `version.txt` file. The purpose of this file is to allow the updater to check whether the latest update version number is the same as the update version number stored locally on the users computer. 

You must make sure that this file does not contain any letters or symbols. Only an integer number. The number `1.3` is not allowed. You are only allowed whole numbers decimal numbers will not work. To be clear, numbers such as `0`, `1`, `2`, `10`, `20`, `100`, `438765` are the kind of numbers you should enter in to this text file.

Enter number `1` in to the **version.txt** file. Then save and close it.

**Note:** You must update this txt file every time you have a new update. The number does not increment automatically.

### Step 4

Now that you have created the version.txt, put all your files that need to be updated in to a zip file. Call it `version1.zip`. Upload it to the same update directory on your web server.

AppUpdate will override every file when it downloads and extracts.

You must update the number **1** incrementation every time you upload a new version file in to the update directory.

When you release the second update, in this case **version.txt** needs to have the value 2 and the new zip file will need to be called **version2.zip**.

### Things to Keep in mind
  - It is not recommended to remove the previous version zip file (Eg. version1.zip) as users will not be able to download the previous release before the newest.
  - Only place a file that should be updated or the update size will be too large which may not be appriciated by your users.


### Error Codes
    
    01 - Missing file
           update.bin is missing, you must put it in to the same diretory as AppUpdate.exe

    23 - Corrupted file
           FileManager.cs cannot read the file. It is corrupted. Update.bin has most likely been tampered

    24 - File does not exist
           FileManager.cs cannot open file or does not have permissions to access it.

    25 - File exists
           Filemanager.cs cannot create a new file because it exists

    26 - File in use
           FileManager.cs cannot write to file because it is currently in use and/or has no access.








