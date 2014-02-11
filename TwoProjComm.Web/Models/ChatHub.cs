using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TwoProjComm.Web.Models
{
    public class ChatHub : Hub
    {
        // thread için özel bir collection yapısıdır. Bu veriyapısında online olan kullanıcıların
        // bir listesini tutacağız.
        static ConcurrentDictionary<string, string> dic = new ConcurrentDictionary<string, string>();

        public void Send(string name, string message)
        {
            // tüm istemcileri güncellemek için
            // bu metodu kullanacağız.
            Clients.All.broadcastMessage(name, message);
        }

        // seçilen istemciye direk olarak özel mesaj yollamak için
        // bu metodu kullanıyoruz.
        public void SendToSpecific(string name, string message, string to)
        {
            Clients.Caller.broadcastMessage(name, message);
            Clients.Client(dic[to]).broadcastMessage(name, message);
        }

        // yeni giren kullanıcının ismi diğer kullanıcılardan 
        // birine aitse tekrardan isim girmesi için uyarı alacak
        public void Notify(string name, string id)
        {
            // isim zaten varsa tekrar gir
            if (dic.ContainsKey(name))
            {
                Clients.Caller.differentName();
            }
            else
            {
                // kullanıcı yoksa ekle
                dic.TryAdd(name, id);

                // online kullanıcı listesini yenile
                foreach (KeyValuePair<string, string> item in dic)
                {
                    Clients.Caller.online(item.Key);
                }

                // diger kullanıcılara bildirim yolla
                // ve giren kullanıcıyı kullanıcılar 
                // listesine ekle
                Clients.Others.enters(name);
            }
        }

        public override Task OnDisconnected()
        {
            var name = dic.FirstOrDefault(x => x.Value == Context.ConnectionId.ToString());
            string s;
            dic.TryRemove(name.Key, out s);

            // kullanıcının çıktıgını tüm kullanıcılara soyle
            return Clients.All.disconnected(name.Key);
        }
    }
}