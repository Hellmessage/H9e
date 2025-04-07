# `H9e` 项目文档
## 概述
`H9e` 是一个用于C#快捷开发的类库,包含Tcp协议,Http客户端,Database等功能,可以快速开发一些常用的功能,如爬虫,数据分析等.
- `H9e.Core` 主要用于一些常用的功能
- `H9e.HttpClient` 主要用于Http协议的快速开发
- `H9e.Tcp` 主要用于TCP协议的快速开发
- `H9e.ProcessModule` 主要用于进程模块的快速开发

---

## `H9e.HttpClient` 使用文档
### 1. H9eHttpClient
```csharp
//创建HttpClient
H9eHttpClient client = new H9eHttpClient();
//设置请求头
client.SetHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
client.Get("http://www.example.com");
```
---

## `H9e.Tcp` 使用文档
### 1. 初始化
```csharp
//初始化
H9eTcpUtils.Init();
//对包进行注册
H9eTcpPacket.RegisterPacket<T>("对应的Tag");
//例如
H9eTcpPacket.RegisterPacket<FileTcpPacket>("file");
```
### 2.创建服务器
```csharp
//创建服务器
H9eTcpServer server = new H9eTcpServer(port);
//设置服务器的回调函数
server.OnPacketMessage += (client, packet) => {
	//处理接收到的包
	if (packet is FileTcpPacket filePacket)
	{
		//处理文件包
		filePacket.SaveFile("文件保存路径");
	}
};
//启动服务器
server.Start(backlog);
```
### 3.创建客户端
```csharp
//创建客户端
H9eTcpClient client = new H9eTcpClient(ip, port);
//接收包
client.OnPacketMessage += (client, packet) => {
	//处理接收到的包
	if (packet is FileTcpPacket filePacket)
	{
		//处理文件包
		filePacket.SaveFile("文件保存路径");
	}
};
//连接服务器
client.Connect();
//自动重连
client.AutoConnect();
//发送包
client.Send(FileTcpPacket.Create("文件路径"));
```

---

## `H9e.Core` 使用文档