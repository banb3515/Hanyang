#region API 참조
using System;
using System.Reflection;
using System.Runtime.Serialization;
#endregion

namespace TcpData
{
    sealed class AllowAllAssemblyVersionsDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            string currentAssembly = Assembly.GetExecutingAssembly().FullName;

            assemblyName = currentAssembly;

            Type typeToDeserialize = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
            return typeToDeserialize;
        }
    }
}
