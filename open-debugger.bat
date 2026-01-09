@echo off
echo Opening Chrome DevTools for React Native debugging...
start chrome "chrome-devtools://devtools/bundled/inspector.html?experiments=true&v8only=true&ws=192.168.1.97:8081/debugger-proxy?role=debugger&name=Chrome"
echo.
echo Instructions:
echo 1. Start your app with F5 (DEBUG: Client - Mobile App)
echo 2. Run this script
echo 3. In your app, shake device and select "Open JS Debugger"
echo 4. Chrome DevTools will connect automatically
pause