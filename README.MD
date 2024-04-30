# RiffRacerServer (v2)
A Local Server and patcher for the game Riff Racer in CSharp.

This code is based on the Python Riff Racer server: https://github.com/rebane2001/riffapi-local<br>
However this is slightly edited and re-implemented in cSharp to make it easier to run (no dependencies).

# Limitations

This Server does not let you:
 - View the charts / scoreboards
 - Save your best scores or ghost files
 - Play any track with your previous score

However this Server does allow you to:
 - Manually keep track of scores (screenshots, etc)
 - Start the game without an internet connection
 - Play tracks whether old or new at any time
 
# Building
Simply open the csproj file with visual studio, and build the project as normal.<br>
When building, I used `Visual Studio Express 2012` though it should work on newer versions.<br>

# Extracting the source from my executables
To view the source of built versions of this project, use 7ip or resource hacker to extract the .rsrc SOURCE folder.<br>
These files are manually added to each built executable to keep the source available with every build in one file.

# Running the Riff Racer server
To run the server, simply run the file. It will try to find the game.exe and ParseOctane.dll first.<br>
If it fails, it will say that Game.exe was not found and quit. If it was found, it will patch that dll file.<br>
Assuming patching the dll fails (no file, invalid file, or re-patching without a .bak file), it will say and quit.<br>
