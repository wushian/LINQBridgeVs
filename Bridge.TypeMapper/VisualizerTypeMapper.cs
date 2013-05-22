﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.DebuggerVisualizers;

namespace LINQBridge.TypeMapper
{
    /// <summary>
    /// Maps all the types of a given assembly to the type T of the debugger visualizer 
    /// </summary>
    /// <typeparam name="T">The debugger Visualizer</typeparam>
    public class VisualizerTypeMapper<T>
        where T : DialogDebuggerVisualizer
    {
        private readonly string _targetVisualizerAssembly;
        //private readonly VisualStudioVersion _visualStudioVersion;
        private readonly VisualizerAttributeInjector _visualizerAttributeInjector;

        private static string TAssemblyLocation { get { return typeof(T).Assembly.Location; } }

        private static string TManifestModuleName { get { return (typeof(T).Assembly.ManifestModule).Name; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizerTypeMapper{T}"/> class.
        /// </summary>
        /// <param name="targetAssemblyToMap">The target assembly to Map with the Visualizer.</param>
        /// <param name="visualizerDescriptionName">Visualizer description.</param>
        public VisualizerTypeMapper(string targetAssemblyToMap, string visualizerDescriptionName)
        {
            _targetVisualizerAssembly = targetAssemblyToMap;
            _visualizerAttributeInjector = new VisualizerAttributeInjector(TAssemblyLocation, _targetVisualizerAssembly, visualizerDescriptionName);

        }

        private static bool IsAlreadyDeployed(string location)
        {
            return File.Exists(location);
        }

        /// <summary>
        /// Creates the specified types to exclude.
        /// </summary>
        public void Create( )
        {
            _visualizerAttributeInjector.MapSystemType(typeof(Dictionary<,>));
            _visualizerAttributeInjector.MapSystemType(typeof(List<>));
            _visualizerAttributeInjector.MapSystemType(typeof(IEnumerable<>));
            _visualizerAttributeInjector.MapSystemType(typeof(IOrderedEnumerable<>));

            _visualizerAttributeInjector.MapTypesFromAssembly();
        }

        /// <summary>
        /// Saves the specified debugger visualizer assembly to a given Path.
        /// </summary>
        /// <param name="debuggerVisualizerPath">The debugger visualizer assembly location.</param>
        private void Save(string debuggerVisualizerPath)
        {
            var debuggerVisualizerAssemblyLocation = debuggerVisualizerPath + TManifestModuleName;

            if (!Directory.Exists(debuggerVisualizerPath))
                Directory.CreateDirectory(debuggerVisualizerPath);

            if (IsAlreadyDeployed(debuggerVisualizerAssemblyLocation))
                //Get all the custom attributes that map other type and import them into the current visualizer
                _visualizerAttributeInjector.SyncronizeMappedTypes(debuggerVisualizerAssemblyLocation);

            // _visualizerAttributeInjector.SyncronizeDebuggerVisualizerVersion(_visualStudioVersion);

            _visualizerAttributeInjector.SaveDebuggerVisualizer(debuggerVisualizerAssemblyLocation);
        }

        /// <summary>
        /// Saves the specified debugger visualizer to a given set of Paths.
        /// </summary>
        /// <param name="debuggerVisualizerPaths">The debugger visualizer Paths.</param>
        public void Save(IEnumerable<string> debuggerVisualizerPaths)
        {
            foreach (var debuggerVisualizerPath in debuggerVisualizerPaths)
            {
                Save(debuggerVisualizerPath);
            }
        }

    }
}
