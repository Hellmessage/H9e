# `H9e` ��Ŀ�ĵ�
## ����
`H9e` ��һ������C#��ݿ��������,����TcpЭ��,Http�ͻ���,Database�ȹ���,���Կ��ٿ���һЩ���õĹ���,������,���ݷ�����.
- `H9e.Core` ��Ҫ����һЩ���õĹ���
- `H9e.HttpClient` ��Ҫ����HttpЭ��Ŀ��ٿ���
- `H9e.Tcp` ��Ҫ����TCPЭ��Ŀ��ٿ���
- `H9e.ProcessModule` ��Ҫ���ڽ���ģ��Ŀ��ٿ���


## `H9e.Tcp` ʹ���ĵ�
### 1. ��ʼ��
```csharp
//��ʼ��
H9eTcpUtils.Init();
//�԰�����ע��
H9eTcpPacket.RegisterPacket<T>("��Ӧ��Tag");
//����
H9eTcpPacket.RegisterPacket<FileTcpPacket>("file");
```
### 2.����������
```csharp
//����������
H9eTcpServer server = new H9eTcpServer(port);
//���÷������Ļص�����
server.OnPacketMessage += (client, packet) => {
	//������յ��İ�
	if (packet is FileTcpPacket filePacket)
	{
		//�����ļ���
		filePacket.SaveFile("�ļ�����·��");
	}
};
//����������
server.Start(backlog);
```
### 3.�����ͻ���
```csharp
//�����ͻ���
H9eTcpClient client = new H9eTcpClient(ip, port);
//���հ�
client.OnPacketMessage += (client, packet) => {
	//������յ��İ�
	if (packet is FileTcpPacket filePacket)
	{
		//�����ļ���
		filePacket.SaveFile("�ļ�����·��");
	}
};
//���ӷ�����
client.Connect();
//�Զ�����
client.AutoConnect();
//���Ͱ�
client.Send(FileTcpPacket.Create("�ļ�·��"));
```
