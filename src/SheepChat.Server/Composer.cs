using System.Composition;
using System.Composition.Hosting;
using System.Composition.Convention;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using SheepChat.Server.Sessions;

namespace SheepChat.Server
{
    public static class Composer
    {
        static readonly ContainerConfiguration configuration;
        static readonly ConventionBuilder conventions;

        static Composer()
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll", SearchOption.AllDirectories);
            var sel = files.Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);
            configuration = new ContainerConfiguration().WithAssemblies(sel.ToList());

            conventions = new ConventionBuilder();
            conventions.ForTypesDerivedFrom(typeof(SessionState)).Export(x => x.AsContractType(typeof(SessionState)));
        }

        public static void Compose(object obj)
        {
            using (var container = configuration.CreateContainer())
            {
                container.SatisfyImports(obj, conventions);
            }
        }
    }
}
