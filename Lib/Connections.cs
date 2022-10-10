using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lib;

public static class Connections
{
    private const string Message = "ping";
    private const string ReturnMessage = "pong";

    private const int ConnectionAttemptsCount = 5;
    private const int ConnectionDelayBetweenAttemptsMillis = 200;

    private const int DelayBetweenMessages = 2_000;
    
    public static async Task StartConnection(int port, string id)
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var remoteEndPoint = new IPEndPoint(IPAddress.Loopback, port);
        await ConnectWithRetry(socket, remoteEndPoint);

        for (var iterationNumber = 0;; iterationNumber++)
        {
            var currentIterationId = $"{id}, {iterationNumber}";
            await Write(socket, Message, currentIterationId);
            await Read(socket, currentIterationId);
            Thread.Sleep(DelayBetweenMessages);
        }
    }

    private static async Task ConnectWithRetry(Socket socket, IPEndPoint remoteEndPoint)
    {
        for (var attempt = 0; attempt < ConnectionAttemptsCount; attempt++)
        {
            try
            {
                await socket.ConnectAsync(remoteEndPoint);
                return;
            }
            catch (Exception)
            {
                // ignored
            }
            Thread.Sleep(ConnectionDelayBetweenAttemptsMillis);
        }

        throw new ArgumentException("Unable to connect");
    }

    public static async Task StartListening(int port, string id)
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var listenEndPoint = new IPEndPoint(IPAddress.Loopback, port);
        socket.Bind(listenEndPoint);
        socket.Listen(1);

        var clientSocket = await socket.AcceptAsync();

        for (var iterationNumber = 0;; iterationNumber++)
        {
            var currentIterationId = $"{id}, {iterationNumber}";
            await Read(clientSocket, currentIterationId);
            await Write(clientSocket, ReturnMessage, currentIterationId);
        }
    }

    private static async Task Read(Socket socket, string id)
    {
        var buffer = new byte[1024];
        var received = await socket.ReceiveAsync(buffer, SocketFlags.None);
        var message = Encoding.UTF8.GetString(buffer, 0, received);
        Console.WriteLine($"[{id}]: Received message '{message}'");
    }
    
    private static async Task Write(Socket socket, string message, string id)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(bytes, SocketFlags.None);
        Console.WriteLine($"[{id}]: Sent message '{message}'");
    }
}