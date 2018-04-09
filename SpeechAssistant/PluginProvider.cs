using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;

namespace SpeechAssistant
{
    /// <summary>
    /// class to assist with getting extensions (plugins) throught the Managed Extensibility Framework
    /// </summary>
   public class PluginProvider
   {
        /// <summary>
        /// singleton implementation
        /// </summary>
        public static PluginProvider instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new PluginProvider();
                }
                return _instance;
            }
        }
        private static PluginProvider _instance;

        /// <summary>
        /// returns a composition container with parts present in a specific assembly
        /// </summary>
        /// <typeparam name="T">the type's assembly to parse parts from</typeparam>
        /// <param name="pluginDirectory">the directory with the extensions (plugins)</param>
        /// <returns></returns>
        public CompositionContainer getPlugins<T>(string pluginDirectory)
        {
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(pluginDirectory));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(T).Assembly));
            return new CompositionContainer(catalog);
        }

    }
}
