using SheepChat.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace SheepChat.Server
{
    /// <summary>
    /// SheepChat.Server application to handle the starting and stopping of systems, and notifying outside subscribers.
    /// </summary>
    public class Application : ISystemHost, IRecomposable
    {
        /// <summary>
        /// Loosely imported Manager systems.
        /// </summary>
        [ImportMany]
        public IEnumerable<InstanceExporter> AvailableSystems { get; set; }

        /// <summary>
        /// Up-to-date Manager systems being used.
        /// </summary>
        public List<InstanceExporter> Systems { get; set; }

        /// <summary>
        /// Application constructor, composes itself
        /// </summary>
        public Application()
        {
            Recompose();
        }

        /// <summary>
        /// Start all systems.
        /// </summary>
        public void Start()
        {
            foreach (var instance in Systems)
            {
                instance.Instance.SubscribeToSystemHost(this);
                instance.Instance.Start();
            }
        }

        /// <summary>
        /// Stop all systems.
        /// </summary>
        public void Stop()
        {
            foreach (var instance in Systems)
            {
                instance.Instance.Stop();
            }
        }

        /// <summary>
        /// Handle data being sent up from the systems.
        /// </summary>
        /// <param name="sender">Manager system sending the message</param>
        /// <param name="msg">Message details</param>
        public void UpdateSystemHost(ISystem sender, string msg)
        {
            Console.WriteLine("<{0}> [{1}] {2}", DateTime.Now, ((Manager)sender).Name, msg);
        }

        /// <summary>
        /// Compose any loose imports and get all the most up-to-date versions of each system type.
        /// </summary>
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