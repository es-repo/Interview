using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PeerMessenger.Messaging
{
  public interface ITcpPeerMessenger
  {
    bool IsStarted { get; }

    IPEndPoint EndPoint { get; }

    IReadOnlyCollection<TcpClient> TcpClients { get; }

    event EventHandler<IPEndPoint> ClientAccepted;

    event EventHandler<Message> MessageReceived;

    event EventHandler<Message> MessageSent;

    Task StartAsync();

    void Stop();

    Task ConnectToAsync(string host, int port);

    Task ConnectToAsync(IPEndPoint endPoint);

    void DisconnectFrom(IPEndPoint endPoint);

    Task SendMessageAsync(string host, int port, string message);

    Task SendMessageAsync(IPEndPoint endPoint, string message);
  }
}