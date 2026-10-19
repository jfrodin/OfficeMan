@echo off
rem Captures every room and every way text appears, then opens a viewer to look through them:
rem left/right arrows (or the d-pad) to step, Esc to close. Also saves everything on one sheet,
rem art\out\review.png. The capture windows open off-screen, so nothing flashes past.
rem Set GODOT to your Godot 4.6 .NET console executable if it lives somewhere else.
if "%GODOT%"=="" set GODOT=E:\Godot_v4.6-stable_mono_win64\Godot_v4.6-stable_mono_win64_console.exe
set ROOT=%~dp0
set OUT=%ROOT%art\out\review
set LIST=%OUT%\list.txt
set HIDE=--position -4000,-4000
if not exist "%OUT%" mkdir "%OUT%"
del /q "%OUT%\*.png" >nul 2>&1
type nul > "%LIST%"

cd /d "%ROOT%game"
echo Building...
dotnet build -v q -nologo || (echo Build failed. & pause & exit /b 1)
"%GODOT%" --headless --path . --import >nul 2>&1

echo Capturing rooms...
call :room 01 flat 06:50 "Teo's flat at dawn"
call :room 02 stairwell 08:00 "The stairwell, morning"
call :room 03 stairwell_night 23:00 "The stairwell at night"
call :room 04 office 10:00 "The office, morning"
call :room 05 terrace 18:00 "The terrace, early evening"
call :room 06 terrace 22:30 "The terrace at night"
echo Capturing text...
call :play 07 2 "A thought (note), nobody talking to Teo"
call :play 08 4 "Overheard through the wall"
call :play 09 13 "A conversation"
call :play 10 18 "Choices in a conversation"
call :play 11 60 "A conversation in the office"
call :play 12 75 "A thought during a conversation"
call :menu 13 main "The pause menu"
call :menu 14 settings "Settings"

echo Making the sheet...
"%GODOT%" --headless --path . -- --stitch "%ROOT%art\out\review.png" "%OUT%\01.png" "%OUT%\02.png" "%OUT%\03.png" "%OUT%\04.png" "%OUT%\05.png" "%OUT%\06.png" "%OUT%\07.png" "%OUT%\08.png" "%OUT%\09.png" "%OUT%\10.png" "%OUT%\11.png" "%OUT%\12.png" "%OUT%\13.png" "%OUT%\14.png" >nul 2>&1

echo Opening the viewer...
start "" "%GODOT%" --path . -- --review "%LIST%"
exit /b 0

:room
"%GODOT%" %HIDE% --path . -- --seed 11 --room %2 --at %3 --shot "%OUT%\%1.png" >nul 2>&1
>> "%LIST%" echo %1.png;%~4 (%3)
exit /b 0

:play
"%GODOT%" %HIDE% --path . -- --seed 11 --shot "%OUT%\%1.png" --steps %2 >nul 2>&1
>> "%LIST%" echo %1.png;%~3
exit /b 0

:menu
"%GODOT%" %HIDE% --path . -- --seed 11 --shot "%OUT%\%1.png" --steps 30 --menu %2 >nul 2>&1
>> "%LIST%" echo %1.png;%~3
exit /b 0
