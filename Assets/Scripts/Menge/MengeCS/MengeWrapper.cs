using System;
using System.Runtime.InteropServices;
using fts;

namespace MengeCS
{
    [PluginAttr("Menge/x86_64/MengeCore_d")]
    class MengeWrapper
    {
#if DEBUG
        const string dllFile = "MengeCore_d";
#else
        const string dllFile = "MengeCore";
#endif

        [PluginFunctionAttr("InitSimulator")]
        public static InitSimulator initSimulator = null;
        public delegate bool InitSimulator([MarshalAs(UnmanagedType.LPStr)] String behaveFile,
                                                [MarshalAs(UnmanagedType.LPStr)] String sceneFile,
                                                [MarshalAs(UnmanagedType.LPStr)] String model,
                                                [MarshalAs(UnmanagedType.LPStr)] String pluginPath
                                                );



        [PluginFunctionAttr("UnloadPlugins")]
        public static UnloadPlugins unloadPlugins = null;
        public delegate int UnloadPlugins();

        [PluginFunctionAttr("SetTimeStep")]
        public static SetTimeStep setTimeStep = null;
        public delegate float SetTimeStep(float timestep);

        [PluginFunctionAttr("DoStep")]
        public static DoStep doStep = null;
        public delegate bool DoStep();

        [PluginFunctionAttr("AgentCount")]
        public static AgentCount agentCount = null;
        public delegate uint AgentCount();

        [PluginFunctionAttr("GetAgentPosition")]
        public static GetAgentPosition getAgentPosition = null;
        public delegate bool GetAgentPosition(uint i, ref float x, ref float y, ref float z);

        [PluginFunctionAttr("GetAgentVelocity")]
        public static GetAgentVelocity getAgentVelocity = null;
        public delegate bool GetAgentVelocity(uint i, ref float x, ref float y, ref float z);

        [PluginFunctionAttr("GetAgentOrient")]
        public static GetAgentOrient getAgentOrient = null;
        public delegate bool GetAgentOrient(uint i, ref float x, ref float y);

        [PluginFunctionAttr("GetAgentClass")]
        public static GetAgentClass getAgentClass = null;
        public delegate int GetAgentClass(uint i);

        [PluginFunctionAttr("GetAgentRadius")]
        public static GetAgentRadius getAgentRadius = null;
        public delegate float GetAgentRadius(uint i);

        [PluginFunctionAttr("ExternalTriggerCount")]
        public static ExternalTriggerCount externalTriggerCount = null;
        public delegate int ExternalTriggerCount();

        [PluginFunctionAttr("ExternalTriggerName")]
        public static ExternalTriggerName externalTriggerName = null;
        public delegate IntPtr ExternalTriggerName(int i);

        [PluginFunctionAttr("FireExternalTrigger")]
        public static FireExternalTrigger fireExternalTrigger = null;
        public delegate void FireExternalTrigger([MarshalAs(UnmanagedType.LPStr)] string lpString);

        [PluginFunctionAttr("GetAgentGoal")]
        public static GetAgentGoal getAgentGoal = null;
        public delegate bool GetAgentGoal(uint agentId, ref uint goalId);

        [PluginFunctionAttr("IsStateGoalSelectorExternal")]
        public static IsStateGoalSelectorExternal isStateGoalSelectorExternal = null;
        public delegate bool IsStateGoalSelectorExternal([MarshalAs(UnmanagedType.LPStr)] String stateNamey);

        [PluginFunctionAttr("SetAgentPointGoal")]
        public static SetAgentPointGoal setAgentPointGoal = null;
        public delegate bool SetAgentPointGoal(uint agentId, float x, float y);

        [PluginFunctionAttr("SetStatePointGoalForAgent")]
        public static SetStatePointGoalForAgent setStatePointGoalForAgent = null;
        public delegate bool SetStatePointGoalForAgent([MarshalAs(UnmanagedType.LPStr)] String behaveFile, uint agentId, float x, float y);


        public delegate void AgentChangedStateCallback(int agentId, [MarshalAs(UnmanagedType.LPStr)] String newState);

        [PluginFunctionAttr("SubscribeToAgentChangedStateEvent")]
        public static AgentChangedStateEventHandler subscribeToAgentChangedStateEvent = null;
        public delegate void AgentChangedStateEventHandler(AgentChangedStateCallback callback);


        [PluginFunctionAttr("AddPositionToExternalAgentGenerator")]
        public static AddPositionToExternalAgentGenerator addPositionToExternalAgentGenerator = null;
        public delegate bool AddPositionToExternalAgentGenerator([MarshalAs(UnmanagedType.LPStr)] String generatorName, float x, float y);


        [PluginFunctionAttr("AddPositionOrientationToExternalAgentGenerator")]
        public static AddPositionOrientationToExternalAgentGenerator addPositionOrientationToExternalAgentGenerator = null;
        public delegate bool AddPositionOrientationToExternalAgentGenerator([MarshalAs(UnmanagedType.LPStr)] String generatorName, float positionX, float positionY, float orientationX, float orientationY);

        [PluginFunctionAttr("ClearExternalAgentGeneratorPositions")]
        public static ClearExternalAgentGeneratorPositions clearExternalAgentGeneratorPositions = null;
        public delegate bool ClearExternalAgentGeneratorPositions([MarshalAs(UnmanagedType.LPStr)] String generatorName);

        [PluginFunctionAttr("TriggerExternalAgentGeneratorSpawn")]
        public static TriggerExternalAgentGeneratorSpawn triggerExternalAgentGeneratorSpawn = null;
        public delegate bool TriggerExternalAgentGeneratorSpawn([MarshalAs(UnmanagedType.LPStr)] String generatorName);
    }
}
