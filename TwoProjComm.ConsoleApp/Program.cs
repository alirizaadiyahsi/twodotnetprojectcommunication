using Microsoft.AspNet.SignalR.Client;
using System;

namespace TwoProjComm.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // web uygulamasına bağlanmak için
            var hubConnection = new HubConnection("http://localhost:7412/");

            // web uygulamasındaki proxy olarak kullanacağımız hub
            var chat = hubConnection.CreateHubProxy("ChatHub");

            // "broadcastMessage" metodu ile haberleşme sağlanacak
            chat.On<string, string>("broadcastMessage", (name, message) =>
            {
                Console.Write(name + ": ");
                Console.WriteLine(message);
            });

            // hub baglantısı başlatılıyor
            // ve baglanana kadar bekletiliyor.
            hubConnection.Start().Wait();

            // console uygulamasının giriş yaptıgına dair uyarı
            chat.Invoke("Notify", "Console Uygulaması", hubConnection.ConnectionId);

            string msg = null;

            // consoldan mesaj geldiği sürece mesajı oku ve
            // tüm istemcilere yolla
            while ((msg = Console.ReadLine()) != null)
            {
                chat.Invoke("Send", "Console Uygulaması", msg).Wait();
            }
        }
    }
}
