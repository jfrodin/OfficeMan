@echo off
rem Starts The Misadventures of Office Man (prototype).
rem Set GODOT to your Godot 4.6 .NET executable if it lives somewhere else.
if "%GODOT%"=="" set GODOT=E:\Godot_v4.6-stable_mono_win64\Godot_v4.6-stable_mono_win64.exe

cd /d "%~dp0game"
echo Building...
dotnet build -v q -nologo || (echo Build failed. & pause & exit /b 1)
if not exist ".godot" "%GODOT%" --headless --path . --import
start "" "%GODOT%" --path .
