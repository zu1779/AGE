namespace Zu1779.AGE.Contract
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class AppDomainTunnel : MarshalByRefObject
    {
        public (string assemblyName, string typeName) ProbeAssemblies(string path, Type probeType)
        {
            var probedTypes = new List<Type>();
            var files = Directory.GetFiles(path, "*.dll");
            foreach (var file in files)
            {
                try
                {
                    var asm = Assembly.LoadFile(file);
                    //resolveDependencies(asm);
                    var types = asm.GetTypes().Where(c => !c.IsInterface && probeType.IsAssignableFrom(c));
                    if (types.Any()) probedTypes.AddRange(types);
                }
                catch (Exception) { }
            }
            if (!probedTypes.Any()) throw new ApplicationException("No environment found");
            if (probedTypes.Count > 1) throw new ApplicationException("More than 1 environment found");
            return (probedTypes[0].Assembly.FullName, probedTypes[0].FullName);
        }

        //private void resolveDependencies(Assembly assembly)
        //{
        //    var referenced = assembly.GetReferencedAssemblies();
        //    if (referenced.Any())
        //    {
        //        foreach (var reference in referenced)
        //        {
        //            var asm = Assembly.Load(reference);
        //            resolveDependencies(asm);
        //        }
        //    }
        //}
    }
}
