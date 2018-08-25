namespace Zu1779.AGE.Contract
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using log4net;
    using Newtonsoft.Json;

    public class AppDomainTunnel : MarshalByRefObject
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AppDomainTunnel));

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
    }
}
