88b           d88    ,ad8888ba,    88888888ba     ,ad8888ba,    
888b         d888   d8"'    `"8b   88      "8b   d8"'    `"8b   
88`8b       d8'88  d8'        `8b  88      ,8P  d8'        `8b  
88 `8b     d8' 88  88          88  88aaaaaa8P'  88          88  
88  `8b   d8'  88  88          88  88""""""8b,  88          88  
88   `8b d8'   88  Y8,        ,8P  88      `8b  Y8,        ,8P  
88    `888'    88   Y8a.    .a8P   88      a8P   Y8a.    .a8P   
88     `8'     88    `"Y8888Y"'    88888888P"     `"Y8888Y"'    

1. Dependencies
2. Running Mobo and MoboServer using supplied .cmd files
3. Running Mobo manually
4. Running MoboServer manually
5. Compiling Mobo and MoboServer yourself
6. Controls
7. SVN directory structure
8. Credits

1______________________________________________________________

Mobo and MoboServer currently only run on Windows Vista and
above. Both depend on the .NET framework 4.0 but this is almost
always present on up to date Windows systems.

The Microsoft XNA Framework Redistributable 4.0 is also
required, but once again is usually a part of regular Windows
updates. If it isn't installed however, the redistributable
can be downloaded and installed at:
https://www.microsoft.com/en-gb/download/details.aspx?id=20914/

Both of the above dependencies are installed on all Windows
machines throughout the University.

2______________________________________________________________

One can run a test scenario by executing the script
"run_demo.cmd" found in the root directory of this project.

The script opens three instances of the client, "Mobo.exe" and
a single instance of the server, "MoboServerWPF.exe".

It may be necessary to reposition the windows opened by the
three "Mobo.exe" instances, as they will all appear at the
centre of the screen.

"run_client.cmd" and "run_server.cmd" run the client and server
individually.

! IMPORTANT !
In some scenarios, i.e. the project folder is on a network
drive (example of this would be "\\uol.le.ac.uk\") neither the
script nor the program will be able to run. In this case it
is necessary to move the project folder to an external drive
such as a USB drive or a local drive that is not networked.
XNA applications will *not* run on network drives due to the
the inability of the content loader to load content from the
network drive, so the program will always crash at startup.

If the script fails to run the client and server, please
proceed to 2. and 3. where it is explained how to run the
client and server manually.

3______________________________________________________________

The pre-compiled binaries for Mobo can be found in the
directory path "code\trunk\Mobo\Mobo\bin\Windows\Release\".

Then execute "Mobo.exe".

Once again, the program will crash on startup if it is located
on a network drive. Please follow the instructions above if
this occurs.

4______________________________________________________________

The pre-compiled binaries for MoboServerWPF can be found in the
directory path
"code\trunk\Mobo\MoboServerWPF\bin\Windows\Release\".

Then execute "MoboServerWPF.exe".

5______________________________________________________________

Mobo and MoboServerWPF were developed in the same Visual Studio
2015 project, and thus can be built together by right clicking
the solution in Visual Studio and selecting "Batch Build...".

To load the project into Visual Studio, simply import the
project file "Mobo.sln" found at "code\trunk\Mobo". The
solution explorer on the right hand side will display the two
programs Mobo and MoboServerWPF, and can be expanded to display
the classes and assets therein.

6______________________________________________________________

Movement - WASD or ARROW KEYS
Shooting - LEFT MOUSE BUTTON
Exit to menu - ESCAPE

7______________________________________________________________

code\trunk\Mobo - Code directory for client and server.

code\trunk\Mobo\Mobo\ - Code directory for client.

code\trunk\Mobo\MoboServer\ - Code directory for the
depreciated command line server.

code\trunk\Mobo\MoboServerWPF\ - Code directory for current
server using WPF.

code\trunk\Mobo\LidgrenNetwork\ - Library for Lidgren.Network
shared between client and server.

code\trunk\Mobo\Mobo.sln - Visual Studio project file.

other\mobo_assets - Free assets, some of which were used as
graphics in the client.

other\screenshots - Various screenshots and videos captured
during development.


8______________________________________________________________

Project Mobo was developed by Christopher Cola as part of a
final year project at the University of Leicester.

Project Mobo uses the external library Lidgren.Network
https://code.google.com/archive/p/lidgren-network-gen3/
https://github.com/lidgren/lidgren-network-gen3/

Project Mobo uses free to use graphic and sound assets from
http://opengameart.org/