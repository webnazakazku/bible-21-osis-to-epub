# Osis to MySQL

## Download release

- [Latest Release](https://github.com/webnazakazku/bible-21-osis-to-epub/releases)

# Developement 

## Requirements

[.NET SDK](https://www.microsoft.com/net/learn/get-started/windows)

## How to build

```bash
dotnet build bible-21-osis-to-epub.csproj
```

## How to run

Resources can be found [here](https://1drv.ms/f/s!ArZuV_aXyWwR3YNh7fHBjO8WgYfObQ)

- After you build the application you can find it in `./bible-21-osis-to-epub/bin/Debug/`
- Run it by `bible-21-epub.exe PATH_TO_XML_FILE.xml`

## How to debug in Visual Studio

- Open `bible-21-osis-to-epub.sln` in Visual Studio
- In `Solution Explorer` find `Properties` and open it
- In `Debug` tab you can find `Command line arguments`
- Add a path to the xml file
- Run debug