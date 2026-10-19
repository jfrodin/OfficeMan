@echo off
rem Builds the game and makes art\out\review.png: every room, and every way text appears,
rem on one sheet, for checking the look after a change. Takes a minute or two.
rem Set GODOT to your Godot 4.6 .NET console executable if it lives somewhere else.
if "%GODOT%"=="" set GODOT=E:\Godot_v4.6-stable_mono_win64\Godot_v4.6-stable_mono_win64_console.exe
set ROOT=%~dp0
set OUT=%ROOT%art\out\review
if not exist "%OUT%" mkdir "%OUT%"

cd /d "%ROOT%game"
echo Building...
dotnet build -v q -nologo || (echo Build failed. & pause & exit /b 1)
"%GODOT%" --headless --path . --import >nul 2>&1

echo Rooms...
call :room 01 flat 06:50
call :room 02 stairwell 08:00
call :room 03 stairwell_night 23:00
call :room 04 office 10:00
call :room 13 terrace 18:00
call :room 14 terrace 22:30
echo Text...
call :play 05 2  "Thought (note)"
call :play 06 4  "Overheard"
call :play 07 13 "Conversation"
call :play 08 18 "Choices"
call :play 09 60 "Conversation in the office"
call :play 10 75 "Thought during a conversation"
"%GODOT%" --path . -- --seed 11 --shot "%OUT%\11.png" --steps 30 --menu main >nul 2>&1
"%GODOT%" --path . -- --seed 11 --shot "%OUT%\12.png" --steps 30 --menu settings >nul 2>&1

echo Sheet...
"%GODOT%" --headless --path . -- --stitch "%ROOT%art\out\review.png" "%OUT%\01.png" "%OUT%\02.png" "%OUT%\03.png" "%OUT%\04.png" "%OUT%\05.png" "%OUT%\06.png" "%OUT%\07.png" "%OUT%\08.png" "%OUT%\09.png" "%OUT%\10.png" "%OUT%\11.png" "%OUT%\12.png" "%OUT%\13.png" "%OUT%\14.png" >nul 2>&1
echo Done: art\out\review.png
exit /b 0

:room
"%GODOT%" --path . -- --seed 11 --room %2 --at %3 --shot "%OUT%\%1.png" >nul 2>&1
exit /b 0

:play
"%GODOT%" --path . -- --seed 11 --shot "%OUT%\%1.png" --steps %2 >nul 2>&1
exit /b 0
