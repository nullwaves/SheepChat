using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using SheepChat.Server.Sessions;

namespace SheepChat.Server
{
    public static class Composer
    {
        public static CompositionContainer Container { get; set; }

        static Composer()
        {
            var asm = Assembly.GetExecutingAssembly();

            var asmCatalog = new AssemblyCatalog(asm);
            var dirCatalog = new DirectoryCatalog(Path.GetDirectoryName(asm.Location));

            Container = new CompositionContainer(
                new AggregateCatalog(
                    asmCatalog,
                    dirCatalog
                    ));
        }

        public static void Compose(object obj)
        {
            Container.ComposeParts(obj);
        }
    }
}
