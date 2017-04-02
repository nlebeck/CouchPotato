# XboxControllerRemote

The goal of this project is to write a program that lets you use your Xbox controller as a remote
control when browsing Netflix, providing a similar experience to browsing Netflix on a game
console. Depending on how things go, this project may be specialized to Netflix, or it may become
a general mouse/keyboard emulator using an Xbox controller.

## Helpful resources that I used for this project

Microsoft's XInput tutorial: https://msdn.microsoft.com/en-us/library/windows/desktop/ee417001(v=vs.85).aspx
This discussion gave me the idea of using mouse\_event() to simulate button clicks, although I had to write a C++/CLI wrapper for it since I couldn't get a DllImport to work as shown: https://www.gamedev.net/topic/321029-how-to-simulate-a-mouse-click-in-c/
