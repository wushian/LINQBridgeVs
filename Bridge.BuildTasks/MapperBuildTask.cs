﻿using LINQBridge.DynamicVisualizers;
using LINQBridge.TypeMapper;
using LINQBridge.VisualStudio;
using Microsoft.Build.Framework;

namespace LINQBridge.BuildTasks
{
    public class MapperBuildTask : ITask
    {
        [Required]
        public string Assembly { private get; set; }

        [Required]
        public string VisualStudioVer { private get; set; }


        public string Resources { private get; set; }

        /// <summary>
        ///     Executes an ITask. It creates a LINQPadDebuggerVisualizer mapping all the types of a given assembly
        /// </summary>
        /// <returns>
        ///     true if the task executed successfully; otherwise, false.
        /// </returns>
        public bool Execute()
        {
            var typeMapper = new VisualizerTypeMapper<DynamicDebuggerVisualizer>(Assembly,
                                                                                 DynamicVisualizers.Properties.Resources
                                                                                            .VisualizerName);

            typeMapper.Create();
            typeMapper.Save(VisualStudioOptions.VisualStudioPaths[VisualStudioVer]);

            return true;
        }

        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }
    }
}