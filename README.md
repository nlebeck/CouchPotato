# XboxControllerRemote

This program lets you use your Xbox controller to browse video streaming services on your PC.
By default, the Xbox controller acts as a mouse, but pressing the back button will put the
controller into "keyboard mode," letting you use an on-screen keyboard to type in things like
search keywords or login information. Currently, the program presents three hard-coded options
for apps/websites to start (Netflix, Hulu, and Steam Big Picture mode), and it always opens
websites in Internet Explorer, but in the future, the available apps and choice of browser will
be customizable.

## Helpful resources that I used for this project

* Microsoft's XInput tutorial: https://msdn.microsoft.com/en-us/library/windows/desktop/ee417001(v=vs.85).aspx
* This discussion gave me the idea of using mouse\_event() to simulate button clicks, although I had to write a C++/CLI wrapper for it since I couldn't get a DllImport to work as shown: https://www.gamedev.net/topic/321029-how-to-simulate-a-mouse-click-in-c/
