using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PeerMessenger.Messaging
{
  public class TcpPeerMessenger : ITcpPeerMessenger
  {
    private const int NextReceivingMessageWaitDelayMs = 200;
    private readonly TcpListener _tcpListener;
    private readonly Dictionary<IPEndPoint, TcpClient> _endPointToTcpClientsMap;
    private readonly IMessageSerializer _messageSerializer;

    public event EventHandler<IPEndPoint> ClientAccepted;
    public event EventHandler<Message> MessageReceived;
    public event EventHandler<Message> MessageSent;

    public bool IsStarted { get; private set; }

    public IPEndPoint EndPoint { get => (IPEndPoint)_tcpListener.LocalEndpoint; }

    public IReadOnlyCollection<TcpClient> TcpClients { get => _endPointToTcpClientsMap.Values; }

    public TcpPeerMessenger(IMessageSerializer messageSerializer, int port = 0)
    {
      _messageSerializer = messageSerializer;
      _tcpListener = TcpListener.Create(port);
      _endPointToTcpClientsMap = new Dictionary<IPEndPoint, TcpClient>();

      ClientAccepted += (s, e) => { };
      MessageReceived += (s, e) => { };
      MessageSent += (s, e) => { };
    }

    public Task StartAsync()
    {
      IsStarted = true;
      _tcpListener.Start();

      return Task.Run(async () =>
      {
        while (IsStarted)
        {
          try
          {
            TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();
            NetworkStream s = tcpClient.GetStream();
            _endPointToTcpClientsMap.Add((IPEndPoint)tcpClient.Client.RemoteEndPoint, tcpClient);
            StartReceivingMessages(tcpClient);

            ClientAccepted(this, (IPEndPoint)tcpClient.Client.RemoteEndPoint);
          }
          catch (ObjectDisposedException)
          {
            break;
          }
        }
      });
    }

    public void Stop()
    {
      IsStarted = false;
      foreach (IPEndPoint endPoint in _endPointToTcpClientsMap.Keys.ToArray())
      {
        DisconnectFrom(endPoint);
      }

      _tcpListener.Stop();
    }

    public Task ConnectToAsync(string host, int port)
    {
      IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(host), port);
      return ConnectToAsync(endPoint);
    }

    public async Task ConnectToAsync(IPEndPoint endPoint)
    {
      if (!IsStarted)
      {
        return;
      }

      if (_endPointToTcpClientsMap.ContainsKey(endPoint))
      {
        return;
      }

      TcpClient tcpClient = new TcpClient();
      await tcpClient.ConnectAsync(endPoint.Address, endPoint.Port);
      _endPointToTcpClientsMap.Add(endPoint, tcpClient);
      StartReceivingMessages(tcpClient);
    }

    public void DisconnectFrom(IPEndPoint endPoint)
    {
      if (!_endPointToTcpClientsMap.ContainsKey(endPoint))
      {
        return;
      }

      _endPointToTcpClientsMap[endPoint].Close();
      _endPointToTcpClientsMap.Remove(endPoint);
    }

    public Task SendMessageAsync(string host, int port, string message)
    {
      IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(host), port);
      return SendMessageAsync(endPoint, message);
    }

    public Task SendMessageAsync(IPEndPoint endPoint, string message)
    {
      TcpClient tcpClient = _endPointToTcpClientsMap.GetValueOrDefault(endPoint);
      if (tcpClient == null || !tcpClient.Connected)
      {
        throw new ArgumentException($"Not connected to {endPoint}", nameof(endPoint));
      }

      NetworkStream networkStream = tcpClient.GetStream();

      string sendString = _messageSerializer.SerializeMessageBody(message) + "\n";
      byte[] sendBytes = Encoding.UTF8.GetBytes(sendString);

      Task task = networkStream.WriteAsync(sendBytes).AsTask();
      MessageSent(this, new Message(EndPoint, message));
      return task;
    }

    private void StartReceivingMessages(TcpClient tcpClient)
    {
      Task.Run(async () =>
      {
        byte[] buffer = new byte[1024];
        NetworkStream networkStream = tcpClient.GetStream();

        while (IsStarted)
        {
          try
          {
            MemoryStream memoryStream = new MemoryStream();
            while (networkStream.DataAvailable)
            {
              int bytesCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
              memoryStream.Write(buffer, 0, bytesCount);
            }
            if (memoryStream.Length > 0)
            {
              string receivedText = Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
              foreach (string line in receivedText.Split('\n', StringSplitOptions.RemoveEmptyEntries))
              {
                string messageBody = _messageSerializer.DeserializeMessageBody(line);
                Message message = new Message((IPEndPoint)tcpClient.Client.RemoteEndPoint, messageBody);
                MessageReceived(this, message);
              }
            }
            System.Threading.Thread.Sleep(NextReceivingMessageWaitDelayMs);
          }
          catch (ObjectDisposedException)
          {
            break;
          }
        }
      });
    }
  }
}