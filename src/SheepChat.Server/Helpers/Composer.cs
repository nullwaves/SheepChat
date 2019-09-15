using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;

namespace SheepChat.Server
{
    /// <summary>
    /// Default static composer class for use in any system.
    /// </summary>
    public static class Composer
    {
        /// <summary>
        /// Container for composing objects in.
        /// </summary>
        public static CompositionContainer Container { get; set; }

        /// <summary>
        /// Default constructor for Composer. Sets the catalogs from which we'll be loading loose imports.
        /// </summary>
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

        /// <summary>
        /// Compose loose imports on an object.
        /// </summary>
        /// <param name="obj">Object with loose imports</param>
        public static void Compose(object obj)
        {
            Container.ComposeParts(obj);
        }
    }
}
