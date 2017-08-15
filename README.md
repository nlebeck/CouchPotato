# XboxControllerRemote

This program lets you use an Xbox controller to control the mouse and keyboard of your PC, so that you can browse streaming
video services or use apps like Skype from the comfort of your couch. The program starts by
letting you choose from a menu of websites and apps (which you can customize), and once you launch
an app, the Xbox controller acts as a mouse to let you navigate the app. You can also go into
"keyboard mode," letting you use an on-screen keyboard to type in things like search keywords or
login information.

## Controls (short version)

Use the D-Pad to navigate menus and press A to select menu items. Once you launch an app, use the
left thumbstick to move the mouse, press A to left click, and press B to right click. Press Start
to simulate pressing the Enter key, and press Back to enter the Escape key. Use the triggers to
simulate scrolling up and down with the mouse wheel. Press Y to switch into and out of Keyboard
Mode.

## Controls

When using a video streaming service, this program operates in two different modes. In "app mode,"
you can use the controller to move and click the mouse and enter certain keyboard keys. In
"keyboard mode," you can enter keyboard input including letters, numbers, and symbols using an
on-screen keyboard. There is also a "mouse emulator mode" available from the app's main menu. In
this mode, the Xbox controller simply acts as a mouse, and keyboard mode is not available, allowing
you to start any program you want or do anything else you can do with just a mouse.

### App mode

* Left thumbstick: move mouse
* A: left click
* B: right click
* X (hold): enable speech recognition for alphanumeric input
* Y: switch to keyboard mode
* D-Pad: left/right/up/down arrow keys
* Start: enter key
* Back: escape key
* Left trigger: mouse wheel scroll up
* Right trigger: mouse wheel scroll down

### Keyboard mode

* Right thumbstick: move keyboard window on screen
* D-Pad: move the cursor on the on-screen keyboard
* A: press selected key
* B: backspace key
* Y: return to app mode
* Start: enter key
* Back: escape key
* Left shoulder: switch to symbols
* Right shoulder: switch to uppercase

### Mouse emulator mode

* Left thumbstick: move mouse
* A: left click
* B: right click
* Y: return to the main menu

### Menu controls

* D-Pad: navigate menu
* A: select menu item

## Streaming website instructions and quirks

This section has instructions and tips on navigating specific video streaming websites. Generally,
you want to think of this program as letting you use the mouse (through the left thumbstick, A and B
buttons, and triggers) as well as a subset of useful keyboard keys (through the Start, Back, and D-Pad buttons).
If you already know how to navigate the website with the mouse and those keys, you're all set. If
you need to type letters and numbers, you can switch to keyboard mode, which most of the websites
support (just make sure you "click" inside of the text box before switching to keyboard mode).

### Amazon Video

Amazon Video works great with this program. When watching a video, just press A to pause/unpause and
use the D-Pad left and right buttons to track forwards and backwards. When browsing movies and TV
shows, use the left thumbstick and A button to control the mouse. Use the triggers to scroll up and
down. To search for a movie or TV show,
just click the cursor inside the search box and switch to keyboard mode (with the Y button) to type
in your search terms.

### Hulu

Hulu also works well with this program. The controls are virtually identical to the Amazon Video
controls described above.

### Netflix

Netflix is a bit more of a pain to use with this program, due to the weird way it handles keyboard
input. When watching a video, you can always use the left thumbstick and A button to control the
video player via the mouse. To pause the video with a button press, you can use the Start button,
but only if your most recent "mouse click" was on the video player's pause/unpause button; the same
is true for using the D-Pad left/right buttons to track forwards and backwards. Also, there is a
glitch that causes the video to occasionally start tracking from an earlier time when you press
the D-Pad, which ends up rewinding the video.

When I watch videos with Netflix, I make sure to "click" the on-screen pause/unpause button when I
first start watching a video, and thereafter only use the start button and D-pad left/right buttons
to control the playback (it's as if I were only using the keyboard). Also, I usually pause before
trying to track with the D-Pad, since that seems to prevent the rewinding glitch mentioned above.

Browsing Netflix is also more of a pain than with the other video streaming services. For reasons
related to details of how the Netflix web app handles keyboard input, this program's keyboard mode
does not work with Netflix. If you have a microphone, you can use the experimental speech
recognition feature to "type" letters into the search box by saying the NATO phonetic alphabet.
Otherwise, you'll have to pull out a keyboard and use that to enter your search terms.

## Customizing the app menu

The apps and websites available in the app menu can be customized by modifying this program's
config file. The config file is an XML file called `Config.xml` located in the same directory as
this program (a default config file will be generated the first time you run this program and
whenever the config file is moved or deleted). The `menuItems` element contains a set of child
elements describing the websites and apps in the app menu. There are three kinds of entries:
* A `website` element describes a website that will be launched in a browser window. The Xbox
controller will replace the mouse and keyboard while browsing this website. Child elements:
    * `name`: The name that will be shown for this entry.
	* `url`: The URL of the website.
* A `program` element describes a mouse-and-keyboard app. When launched with this program, the Xbox
controller will replace the mouse and keyboard, just like with a website. Child elements:
    * `name`: The name that will be shown for this entry.
	* `processName`: The process name of the program.
	* `processPath`: The path to the EXE file used to launch the program.
	* `args`: Any command-line arguments to be passed to the program when launching it. An empty
	element is fine.
	* `appStartedArgs` (optional): Any command-line arguments to be passed to the program when a
	process of that program is already running. If this element is missing, you will not be able
	to launch the program when a process of it is already running. An empty element is fine.
* A `controllerProgram` element describes an app that natively supports an Xbox controller. This
program will ignore the Xbox controller input while the controller-enabled app is running. Child
elements: the same as for a `program` entry (see above).

## Speech recognition mode

For websites like Netflix that don't work with keyboard mode, this program has a speech recognition
mode that uses Windows Speech Recognition to let you enter characters with your voice. To activate
speech recognition mode, hold down the X button. While the X button is held down, to enter a
letter, say the corresponding NATO Phonetic Alphabet
(https://en.wikipedia.org/wiki/NATO_phonetic_alphabet) code word. To enter a numerical digit, just
say the digit's name, and to enter a space or backspace, say "space" or "backspace." Speech
recognition mode will only work if you have a microphone plugged into your computer, and it might
not work perfectly due to the inherent noise and error involved in speech recognition.

## Programming notes

* I use `mouse_event()` rather than `SendInput()` to spoof mouse input because
mouse click events sent with `SendInput()` were not being applied when another
application had the focus. Given that `mouse_event()` is deprecated, I'm not
sure why it works and `SendInput()` doesn't. It's possible that there is a
UIPI privilege issue with `SendInput()`, but I would think that any such issue
would apply to `mouse_event()` as well.

### Helpful resources that I used

* Microsoft's XInput tutorial:
https://msdn.microsoft.com/en-us/library/windows/desktop/ee417001(v=vs.85).aspx
* This discussion gave me the idea of using `mouse_event()` to simulate button clicks, although I
had to write a C++/CLI wrapper for it since I couldn't get a DllImport to work as shown:
https://www.gamedev.net/topic/321029-how-to-simulate-a-mouse-click-in-c/
* Shutting down the computer from inside of a C# program:
http://stackoverflow.com/questions/102567/how-to-shut-down-the-computer-from-c-sharp
