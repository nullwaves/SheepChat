using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace SheepChat.Server
{
    public static class Composer
    {
         static readonly ContainerConfiguration configuration;

        static Composer()
        {
            configuration = new ContainerConfiguration().WithAssemblies(
                Directory.GetFiles(Assembly.GetEntryAssembly().Location, "*.dll", SearchOption.AllDirectories)
                         .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                         .ToList()
                         );
        }

        public static void Compose(object obj)
        {
            using (var container = configuration.CreateContainer())
            {
                container.SatisfyImports(obj);
            }
        }
    }
}
