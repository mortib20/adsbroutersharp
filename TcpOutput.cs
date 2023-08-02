using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ADSBRouterSharpv2
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

        public async Task SendAsync(byte[] buffer)
        {
            if (isReconnecting) return;

            if (!_client.Connected)
            {
                _ = Reconnect();
            }

            try
            {
                await _client.Client.SendAsync(buffer);
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