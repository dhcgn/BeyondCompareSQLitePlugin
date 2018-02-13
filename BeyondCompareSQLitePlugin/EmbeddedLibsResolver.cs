using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BeyondCompareSQLitePlugin
{
    public class EmbeddedLibsResolver
    {
        public static void Init()
        {
            var assemblies = new Dictionary<String, Assembly>();
            var executingAssembly = Assembly.GetExecutingAssembly();
            var resources = executingAssembly.GetManifestResourceNames()
                .Where(n => n.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase));

            foreach (var resource in resources)
            {
                using (var stream = executingAssembly.GetManifestResourceStream(resource))
                using (var memstream = new MemoryStream())
                {
                    stream.CopyTo(memstream);

                    assemblies.Add(resource, Assembly.Load(memstream.ToArray()));
                }
            }

            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                var assemblyName = new AssemblyName(e.Name);
                var path = $"{assemblyName.Name}.dll";

                Console.Out.WriteLine($"AssemblyResolve: {path}");
                
                return assemblies.ContainsKey(path) ? assemblies[path] : null;
            };
        }
    }
}