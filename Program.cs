using System.Net;

namespace ADSBRouterSharp
{
    internal class Program
    {
        static readonly CancellationTokenSource cancelSource = new();
        static readonly CancellationToken cancel = cancelSource.Token;

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += StrgCHandler;
            var config = TcpConfig.FromFile("config.json");
            var input = TcpInput.CreateInput(IPAddress.Parse(config.Input.Host), config.Input.Port);
            var outputs = config.Outputs.Select(e => TcpOutput.CreateOutput(e.Host, e.Port)).ToList();

            // Connect outputs
            outputs.ForEach(async o => await o.Reconnect());

            // Register Event to send data to outputs
            input.NewData += (buffer, length) => outputs.ForEach(x => x.SendAsync(buffer, length));

            // Start the input and accepts one client
            input.Start();
            await input.AcceptClientsAsync(cancel);
            input.Stop();

            outputs.ForEach(x => x.Close());
        }

        static void StrgCHandler(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            cancelSource.Cancel();
        }
    }
}