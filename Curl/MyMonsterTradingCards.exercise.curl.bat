@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo.

REM --------------------------------------------------
echo 1) Create Users (Registration)
REM Create User
curl -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\", \"Role\":\"admin\", \"Name\":\"Daniel Kienboec\", \"Email\":\"kienboec@gmail.com\"}"
curl -X POST http://localhost:10001/users --header "Content-Type: application/json" -d "{\"Username\":\"altenhof\", \"Password\":\"markus\", \"Role\":\"admin\", \"Name\":\"Markus Altenhof\", \"Email\":\"markus@gmail.com\"}"
