using Owin;

namespace TwoProjComm.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // connection ve ya hub ilk bağlandığı zaman bu metod çalışmalı.
            app.MapSignalR();
        }
    }
}