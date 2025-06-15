# CustomEvent
A TShock plugin library for creating custom events in Terraria. </br>
**Important Note**:
- CustomEvent is still in development at the moment so some bugs are to be expected. Feel free to report them via the [Issues](https://github.com/sors89/CustomEvent/issues) tab.</br>
- **As of the current version (v1.0.1), be sure to back up your world file, as this version changes the way the game loads and saves world file.**
# Installation
Like many other TShock plugins, simply place `CustomEvent.dll` alongside `ExHooks.dll` in the `ServerPlugins` folder.
# Troubleshooting
- If you encounter a world-related crash, make sure to remove `CustomEvent.dll` first before loading the backup. After that, create a Github issues ticket to report the bug, including detailed steps on how to reproduce it.
- If you encounter a startup crash, it's likely due to the following reasons:
  - Forgot to include `ExHooks.dll` in the `ServerPlugins` folder.
  - Using a modified version of OTAPI or TShock.
	- You're **fully responsible** for maintaining and fixing any issues in the plugin yourself.
  - Incompatibility with other plugins.
  - The plugin hasn't been updated to the **lastest stable** release of OTAPI or TShock.
# How does this works?
By using IL editing technique to hook into Terraria's event code, we can manipulate it to execute our custom event as well! </br>
# State of development
Development is going slowly right now since this plugin isn’t my main focus.
I’ve got other things going on in real life, so I can only work on it when I have a bit of free time which isn’t often.</br>
# Upcoming features
- Provides a clear indication of event progress.
- Supports event with multi waves.
- Better API.
# ちなみに
- This plugin **requires** [ExHooks](https://github.com/sors89/ExHooks), another plugin library that provides extra hooks which might be useful for your TShock modding.
- This plugin is designed to offer a way to create custom event, it does **not** to provide any built-in events.
- You can use TShock's build-in `/worldevent` command as an alternative way to start a custom event. 
- This is my first time creating a library for others to use so there may be some imperfections. Any suggestions are greatly appreciated!
- The `ExampleEvent` folder contains a sample custom event, feel free to check it out, it's pretty cool, seriously.
- Thanks for reading and have a good day ≧∇≦.