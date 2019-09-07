using SheepChat.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Composition;

namespace SheepChat.Server
{
    public class Application : ISystemHost, IRecomposable
    {

        [ImportMany]
        public IEnumerable<InstanceExporter> Systems { get; set; }

        public Application()
        {
            Recompose();
        }

        public void Start()
        {
            foreach(var instance in Systems)
            {
                instance.Instance.SubscribeToSystemHost(this);
                instance.Instance.Start();
            }
        }

        public void Stop()
        {
            foreach(var instance in Systems)
            {
                instance.Instance.Stop();
            }
        }


        public void UpdateSystemHost(ISystem sender, string msg)
        {
            Console.WriteLine("<{0}> [{1}] {2}", DateTime.Now, ((Manager)sender).Name, msg);
        }

        public void Recompose()
        {
            Composer.Compose(this);
        }
    }
}
