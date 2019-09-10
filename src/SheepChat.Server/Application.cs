using SheepChat.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SheepChat.Server
{
    public class Application : ISystemHost, IRecomposable
    {

        [ImportMany]
        public IEnumerable<InstanceExporter> AvailableSystems { get; set; }

        public List<InstanceExporter> Systems { get; set; }

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
            Composer.Container.ComposeParts(this);

            var newSystems = new List<InstanceExporter>();
            var uniqueSystemNames = (from s in AvailableSystems select s.InstanceType.FullName).Distinct().ToList();

            foreach (var name in uniqueSystemNames)
            {
                newSystems.Add((from s in AvailableSystems
                                where s.InstanceType.FullName == name
                                orderby s.InstanceType.Assembly.GetName().Version descending
                                select s)
                                .FirstOrDefault());
            }

            Systems = newSystems;
        }
    }
}
