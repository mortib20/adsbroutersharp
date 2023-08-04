using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ADSBRouterSharp
{
    internal class TcpOutput
    {
        readonly Logger _logger;
        readonly DnsEndPoint _endPoint;
        TcpClient _client;
        bool isReconnecting = false;

        public TcpOutput(string host, int port)
        {
            _client = new();
            _endPoint = new DnsEndPoint(host, port);
            _logger = new($"OUTPUT {_endPoint.Host}:{_endPoint.Port}");
        }

        public async Task Reconnect()
        {
            await Reconnect(1000);
        }

        public async Task Reconnect(int timeout)
        {
            _logger.Info("Reconnecting...");
            isReconnecting = true;
            await Task.Delay(timeout);
            try
            {
                _client = new();
                await _client.ConnectAsync(_endPoint.Host, _endPoint.Port);
                _logger.Info("Connected");
            }
            catch (SocketException ex)
            {
                _logger.Error($"{ex.Message} ({ex.SocketErrorCode})");
                _ = Reconnect(timeout * 2);
            }
            finally
            {
                isReconnecting = false;
            }
        }

        public void Close()
        {
            _logger.Info("Disconnected");
            _client.Close();
        }

        public void SendAsync(byte[] buffer, int length)
        {
            if (isReconnecting) return;

            if (!_client.Connected)
            {
                _ = Reconnect();
            }

            try
            {
                _client.Client.Send(buffer, length, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                _logger.Error($"{ex.Message} ({ex.SocketErrorCode})");
                _ = Reconnect();
            }
        }

        public static TcpOutput CreateOutput(string host, int port)
        {
            return new(host, port);
        }
    }
}