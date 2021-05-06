# Studies.Joystick
Simple virtual joystick implementation on mobile/android, using Monogame.  
This was a for-study project, but this code doesnt really belong to anyone.  
Please, do use it.  

# Inspirations
I first got started on this through https://github.com/Rast1234/YolkaBot .  
Then i found out that he got inspiration from https://github.com/gnomicstudios/GGJ13 .  
Credits goes to them for implementing and tweaking this complicated code, i just made it instanciable and messed around with a few fields.  
## <font color="red">PROJECT STATUS</font>

### Current status: <font color="#83d4eb">ICEBOX</font> 🧊

While the ball looks kinda weird with the camera transformation and there is next to none documentation,  
I've decided to mark the project as mostly done, and i dont intend to be back any time soon.  
I have other projects in mind, this was just a stepping stone.  

## Installation

For developing and building Monogame applications, you will need the following workloads installed in your Visual Studio
- .NET Core cross-platform development - For Desktop OpenGL and DirectX platforms
- Mobile Development with .NET - For Android and iOS platforms (optional, only needed for android development)
- Universal Windows Platform development - For Windows 10 and Xbox UWP platforms
- .Net Desktop Development - For Desktop OpenGL and DirectX platforms to target normal .NET Framework (requires a graphics card that suports opengl 3.3)

For more information regarding Monogame installation,  
follow this link -> https://docs.monogame.net/articles/getting_started/1_setting_up_your_development_environment_windows.html

The mcgb editor is also required for editing the content pipeline, more info in the link above

MGCB Editor is a tool for editing .mgcb files, which are used for building content.
To register the MGCB Editor tool with Windows and Visual Studio 2019, run the following from the Command Prompt.

```dotnet tool install --global dotnet-mgcb-editor```
```mgcb-editor --register```

## Usage
This application must run on an android device or emulator.  
It is compiled to Android 11.0 (R)  
I have so far tested it in a Samsung A01, it works fine but the sticks are kinda of small.  

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## Technologies
This uses the Monogame framework for creating game-related code.
This laso uses the Monogame.Extended libraries, the only motive being the ease of use of their camera and tile render.  
This project is written in C# 8

## License
[MIT](https://choosealicense.com/licenses/mit/)
