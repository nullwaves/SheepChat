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
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll", SearchOption.AllDirectories);
            var sel = files.Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);
            configuration = new ContainerConfiguration().WithAssemblies(sel.ToList());
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
