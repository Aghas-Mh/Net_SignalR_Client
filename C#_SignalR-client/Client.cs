using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using System.Windows;

namespace C__SignalR_client
{
    public class MyClient
    {
        HubConnection connection;
        string _myName;

        public MyClient()
        {
            connection = new HubConnectionBuilder()
            .WithUrl("https://10.10.10.18:8088/chatHub", (opts) =>
            {
                opts.HttpMessageHandlerFactory = (message) =>
                {
                    if (message is HttpClientHandler clientHandler)
                        // always verify the SSL certificate
                        clientHandler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, sslPolicyErrors) => { return true; };
                    return message;
                };
            }).Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }

        public async Task Connect(string name)
        {
            _myName = name;

            connection.On<int>("ReciveID", (id) =>
            {
                Console.WriteLine($"MyID: {id}");
            });

            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Console.WriteLine($"User: {user}, Message: {message}");
            });

            try
            {
                await connection.StartAsync();
                await connection.InvokeAsync("GetID", _myName);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Connect Exeption: {ex}");
            }
        }

        public async Task Send(string message)
        {
            await connection.InvokeAsync("SendMessage", _myName, message);
        }
    }
}
