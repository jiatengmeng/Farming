@echo off
cd protocol
set client_dest_path="..\msg"
for %%i in (*.proto) do protoc.exe --csharp_out=%client_dest_path% %%i
echo success
pause