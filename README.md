# PiaoPiao
一个十多年前的游戏的个人重置版

## 启动游戏

需要先启动服务器，再启动客户端。

### 启动服务器
```bash
MainS\bin\Debug\net8.0-windows\MainS.exe
```

### 启动客户端
```bash
powershell -Command "Start-Process 'C:\cproject\PiaoPiao\MainC\bin\Debug\net8.0-windows\MainC.exe' -WorkingDirectory 'C:\cproject\PiaoPiao'"
```

注意：客户端必须以 `C:\cproject\PiaoPiao` 为工作目录运行，否则无法找到资源文件。
