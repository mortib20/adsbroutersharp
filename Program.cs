using System.Net;

namespace ADSBRouterSharpv2
{
    internal class Program
    {
        static readonly CancellationTokenSource cancelSource = new();
        static readonly CancellationToken cancel = cancelSource.Token;

        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += StrgCHandler;

            var outputs = new List<TcpOutput>()
            {
                new("feed.adsb.lol", 30004),
                new("feed.adsb.fi", 30004),
                new("feed.adsb.one", 64004),
                new("feed.planespotters.net", 30004),
                new("feed1.adsbexchange.com", 30004)
            };

            outputs.ForEach(async x =>
            {
                await x.Reconnect();
            });

            var input = TcpInput.CreateInput(IPAddress.Any, 30004);

            input.NewData += (byte[] buffer) => Console.WriteLine(System.Text.Encoding.UTF8.GetString(buffer));
            input.NewData += async (byte[] buffer) =>
            {
                outputs.ForEach(async x =>
                {
                    await x.SendAsync(buffer);
                });
            };

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