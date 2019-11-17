echo off
REM ZeroMQ Req-Rep pattern example 1
REM One Req and one Reb
REM Author: Manar Ezzadeen
REM Blog  : http://idevhawk.phonezad.com
REM Email : idevhawk@gmail.com

echo this is cd /d %~dp0
start "Server - Replier" cmd /T:8E /k Replier.exe -b tcp://127.0.0.1:5000 -r "#msg# - Reply" -d 0
start "Client - Requester" cmd /T:8F /k Requester.exe -c tcp://127.0.0.1:5000 -m "Request  #nb#" -x 5 -d 1000