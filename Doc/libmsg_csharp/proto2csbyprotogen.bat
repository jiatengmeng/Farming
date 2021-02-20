@echo off
cd protocol
set client_dest_path="..\msg"
for %%i in (*.proto) do protogen.exe %%i --csharp_out=%client_dest_path%
pause 