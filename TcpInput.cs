using System.Net;
using System.Net.Sockets;

namespace ADSBRouterSharpv2
{
    delegate void NewData(byte[] buffer);

    internal class TcpInput
    {
        readonly Logger _logger;
        readonly TcpListener _listener;

        public event NewData NewData;

        public TcpInput(IPAddress address, int port)
        {
            _listener = new(address, port);
            _logger = new($"INPUT {address}:{port}");
        }

        public void Start()
        {
            try
            {
                _logger.Info("Starting");
                _listener.Start();
            }
            catch (SocketException ex)
            {
                _logger.Error($"{ex.Message} ({ex.SocketErrorCode})");
            }
        }

        public void Stop()
        {
            try
            {
                _logger.Info("Stopping");
                _listener.Stop();
            }
            catch (SocketException ex)
            {
                _logger.Error($"{ex.Message} ({ex.SocketErrorCode})");
            }
        }

        public async Task AcceptClientsAsync(CancellationToken cancel)
        {
            try
            {
                while (!cancel.IsCancellationRequested)
                {
                    using var client = await _listener.AcceptTcpClientAsync(cancel);
                    await HandleClient(client, cancel);
                }
            }
            catch (OperationCanceledException) { } // Ignore this
        }

        private async Task HandleClient(TcpClient client, CancellationToken cancel)
        {
            try
            {
                using var stream = client.GetStream();

                _logger.Info($"New client accepted {client.Client.RemoteEndPoint}");

                while (true)
                {
                    byte[] buffer = new byte[2048];
                    int length = await stream.ReadAsync(buffer, cancel);

                    if (length == 0)
                    {
                        break;
                    }

                    NewData?.Invoke(buffer);
                }

                _logger.Info($"Client disconnected {client.Client.RemoteEndPoint}");
            }
            catch (OperationCanceledException) { } // Ignore this
        }

        public static TcpInput CreateInput(IPAddress addr, int port)
        {
            return new(addr, port);
        }
    }
}