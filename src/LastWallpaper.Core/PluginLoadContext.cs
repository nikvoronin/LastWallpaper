using System.Reflection;
using System.Runtime.Loader;

namespace LastWallpaper.Core
{
    public class PluginLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver _resolver;

        public PluginLoadContext( string pluginPath )
        {
            _resolver = new AssemblyDependencyResolver( pluginPath );
        }

        protected override Assembly? Load( AssemblyName assemblyName )
        {
            string? assemblyPath = _resolver.ResolveAssemblyToPath( assemblyName );

            return
                assemblyPath is not null
                ? LoadFromAssemblyPath( assemblyPath )
                : null;
        }

        protected override IntPtr LoadUnmanagedDll( string unmanagedDllName )
        {
            string? libraryPath = _resolver.ResolveUnmanagedDllToPath( unmanagedDllName );

            return
                libraryPath is not null
                ? LoadUnmanagedDllFromPath( libraryPath )
                : IntPtr.Zero;
        }

        public static Assembly LoadPluginFromFile( string path )
        {
            PluginLoadContext loadContext = new( path );

            return 
                loadContext.LoadFromAssemblyName(
                    new AssemblyName( Path.GetFileNameWithoutExtension( path ) ) );
        }
    }

}
