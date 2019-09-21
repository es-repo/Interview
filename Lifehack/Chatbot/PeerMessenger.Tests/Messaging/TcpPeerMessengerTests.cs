using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PeerMessenger.Messaging.Tests
{
  public class TcpPeerMessengerTests
  {
    [Fact]
    public async Task TestStartStop()
    {
      TcpPeerMessenger messenger = new TcpPeerMessenger(new MessageSerializer());
      Assert.False(messenger.IsStarted);
      Task messengerStartTask = messenger.StartAsync();
      Assert.NotEqual(0, messenger.EndPoint.Port);
      Assert.True(messenger.IsStarted);
      await Task.Delay(300);
      messenger.Stop();
      Assert.False(messenger.IsStarted);
      await messengerStartTask;
      Assert.False(messenger.IsStarted);
    }

    [Fact]
    public void TestConnecting()
    {
      TcpPeerMessenger messenger1 = new TcpPeerMessenger(new MessageSerializer());
      TcpPeerMessenger messenger2 = new TcpPeerMessenger(new MessageSerializer());
      TcpPeerMessenger messenger3 = new TcpPeerMessenger(new MessageSerializer());

      async Task testAction()
      {
        int acceptedClientsCount = 0;
        messenger3.ClientAccepted += (s, a) => acceptedClientsCount++;

        IPEndPoint endPoint3 = GetMessengerEndPoint(messenger3);
        await messenger1.ConnectToAsync(endPoint3);
        await messenger2.ConnectToAsync(endPoint3);

        await Task.Delay(300);

        Assert.Equal(2, acceptedClientsCount);
        Assert.Equal(2, messenger3.TcpClients.Count);
        Assert.Equal(1, messenger1.TcpClients.Count);
        Assert.Equal(1, messenger2.TcpClients.Count);
        Assert.Equal(endPoint3.Port, ((IPEndPoint)(messenger1.TcpClients.First().Client.RemoteEndPoint)).Port);
      }

      StartMessengersAndExecuteActionWhenAllStarted(new[] { messenger1, messenger2, messenger3 }, testAction);
    }

    [Fact]
    public async Task TestSendingMessageToNotConnectedPeer()
    {
      TcpPeerMessenger messenger = new TcpPeerMessenger(new MessageSerializer());
      Task startTask = messenger.StartAsync();
      try
      {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
          await messenger.SendMessageAsync("127.0.0.1", 19999, "Hello");
        });
      }
      finally
      {
        messenger.Stop();
        await startTask;
      }
    }

    [Fact]
    public void TestCommunication()
    {
      TcpPeerMessenger messenger1 = new TcpPeerMessenger(new MessageSerializer());
      TcpPeerMessenger messenger2 = new TcpPeerMessenger(new MessageSerializer());

      async Task testAction()
      {
        string messageTextSendBy1 = "Hi";
        string messageTextSendBy2 = "Hello";
        IPEndPoint endPoint1 = null;

        messenger2.ClientAccepted += (s, e) => endPoint1 = e;

        Message messageReceivedFrom1 = null;
        messenger2.MessageReceived += (s, msg) => messageReceivedFrom1 = msg;

        Message messageReceivedFrom2 = null;
        messenger1.MessageReceived += (s, msg) =>
        {
          messageReceivedFrom2 = msg;
        };

        IPEndPoint endPoint2 = GetMessengerEndPoint(messenger2);
        await messenger1.ConnectToAsync(endPoint2);
        await messenger1.SendMessageAsync(endPoint2, messageTextSendBy1);
        await messenger2.SendMessageAsync(endPoint1, messageTextSendBy2);

        await Task.Delay(300);

        Assert.Equal(messageReceivedFrom1.Body, messageTextSendBy1);
        Assert.Equal(messageReceivedFrom2.Body, messageTextSendBy2);
      }

      StartMessengersAndExecuteActionWhenAllStarted(new[] { messenger1, messenger2 }, testAction);
    }

    private static void StartMessengersAndExecuteActionWhenAllStarted(TcpPeerMessenger[] messengers, Func<Task> action)
    {
      List<Task> messengerStartTasks = new List<Task>();

      foreach (TcpPeerMessenger m in messengers)
      {
        Task t = m.StartAsync();
        messengerStartTasks.Add(t);
      }

      try
      {
        action();
      }
      finally
      {
        foreach (TcpPeerMessenger m in messengers)
        {
          m.Stop();
        }

        Task.WaitAll(messengerStartTasks.ToArray());
      }
    }

    private static IPEndPoint GetMessengerEndPoint(TcpPeerMessenger messenger)
    {
      return new IPEndPoint(IPAddress.Parse("127.0.0.1"), messenger.EndPoint.Port);
    }
  }
}