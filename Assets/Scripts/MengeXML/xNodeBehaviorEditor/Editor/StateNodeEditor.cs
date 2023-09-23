using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using XNode;
using XNodeEditor;
using Menge.BFSM;

[CustomNodeEditor(typeof(StateNode))]
public class StateNodeEditor : NodeEditor
{
    private StateNode m_StateNode;
    private bool m_IsCollapsed = false;

    public override void OnBodyGUI()
    {
        if (m_StateNode == null) m_StateNode = target as StateNode;

        // SerializedObject serializedObject = new SerializedObject(m_GoalSetNode);
        serializedObject.Update();

        m_IsCollapsed = EditorGUILayout.Toggle("Collapse", m_IsCollapsed);


        NodeEditorGUILayout.PortField(new GUIContent("Input Transition"), m_StateNode.GetPort("inTransition"));

        m_StateNode.state.name = EditorGUILayout.TextField("Name", m_StateNode.state.name);

        if (!m_IsCollapsed) m_StateNode.state.final = EditorGUILayout.Toggle("Final", m_StateNode.state.final);

        if (!m_IsCollapsed)
        {

            // Allows Adding a new action of the specified type
            m_StateNode.selectedAddActionType = (ActionTypes)EditorGUILayout.EnumPopup("Action to Add", m_StateNode.selectedAddActionType);

            // Draw GUI
            NodeEditorGUILayout.DynamicPortList(
                "state.actions", // field name
                typeof(Action), // field type
                serializedObject, // serializable object
                NodePort.IO.Input, // new port i/o
                Node.ConnectionType.Override, // new port connection type
                Node.TypeConstraint.None,
                OnCreateReorderableList); // onCreate override. This is where the magic happens.
        }
        // NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("state.goalSelector"), true);


        showGoalSelectorPicker();
        showVelocityComponentPicker();

        if (!m_IsCollapsed)
        {
            showVelocityModifierPicker();
        }
        
        NodeEditorGUILayout.PortField(new GUIContent("Output State"), m_StateNode.GetPort("outState"));

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

    private void showVelocityModifierPicker()
    {
        if (!m_StateNode.HasPort("state.velocityModifier"))
        {
            m_StateNode.AddDynamicInput(typeof(VelocityModifier), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "state.velocityModifier");
        }
        NodeEditorGUILayout.PortField(new GUIContent("Velocity Modifier"), m_StateNode.GetPort("state.velocityModifier"));
    }

    private void showVelocityComponentPicker()
    {
        // Allows Adding a new action of the specified type
        m_StateNode.selectedVelocityComponentType = (VelocityComponentTypes)EditorGUILayout.EnumPopup("Velocity Component Selector", m_StateNode.selectedVelocityComponentType);

        switch (m_StateNode.selectedVelocityComponentType)
        {
            case (VelocityComponentTypes.NONE):
                {
                    if (m_StateNode.state.velocityComponent != null) m_StateNode.state.velocityComponent = null;
                    serializedObject.Update();
                    break;
                }
            case (VelocityComponentTypes.CONSTANT):
                {
                    if (m_StateNode.state.velocityComponent?.GetType() != typeof(ConstantVelocityComponent))
                    {
                        m_StateNode.state.velocityComponent = new ConstantVelocityComponent();
                        serializedObject.Update();
                    }
                    EditorGUILayout.LabelField("Constant Velocity Component Parameters");
                    EditorGUILayout.BeginHorizontal();
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("state.velocityComponent.x"));
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("state.velocityComponent.y"));
                    EditorGUILayout.EndHorizontal();
                    break;
                }
            case (VelocityComponentTypes.CONSTANT_DIRECTION):
                {
                    if (m_StateNode.state.velocityComponent?.GetType() != typeof(ConstantDirectionVelocityComponent))
                    {
                        m_StateNode.state.velocityComponent = new ConstantDirectionVelocityComponent();
                        serializedObject.Update();
                    }
                    EditorGUILayout.LabelField("Constant Direction Velocity Component Parameters");
                    EditorGUILayout.BeginHorizontal();
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("state.velocityComponent.x"));
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("state.velocityComponent.y"));
                    EditorGUILayout.EndHorizontal();
                    break;
                }
            case (VelocityComponentTypes.ZERO):
                {
                    if (m_StateNode.state.velocityComponent?.GetType() != typeof(ZeroVelocityComponent))
                    {
                        m_StateNode.state.velocityComponent = new ZeroVelocityComponent();
                        serializedObject.Update();
                    }
                    EditorGUILayout.LabelField("Always returns Zero velocity");
                    break;
                }
            case (VelocityComponentTypes.GOAL):
                {
                    if (m_StateNode.state.velocityComponent?.GetType() != typeof(GoalVelocityComponent))
                    {
                        m_StateNode.state.velocityComponent = new GoalVelocityComponent();
                        serializedObject.Update();
                    }
                    EditorGUILayout.LabelField("Returns Velocity towards goal");
                    break;
                }
            case (VelocityComponentTypes.VELOCITY_FIELD):
                {
                    if (m_StateNode.state.velocityComponent?.GetType() != typeof(VelocityFieldVelocityComponent))
                    {
                        m_StateNode.state.velocityComponent = new VelocityFieldVelocityComponent();
                        serializedObject.Update();
                    }
                    VelocityFieldVelocityComponent m_velocityFieldVelocityComponent = m_StateNode.state.velocityComponent as VelocityFieldVelocityComponent;
                    EditorGUILayout.LabelField("Velocity Field Velocity Component Parameters");

                    EditorGUILayout.BeginHorizontal();
                    m_velocityFieldVelocityComponent.fileName = EditorGUILayout.TextField("File Name", m_velocityFieldVelocityComponent.fileName);
                    if (GUILayout.Button("Select"))
                    {
                        m_velocityFieldVelocityComponent.fileName = Path.GetFileName(EditorUtility.OpenFilePanel("Velocity Field File", "", ""));
                        serializedObject.Update();
                    }
                    EditorGUILayout.EndHorizontal();

                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("state.velocityComponent.useNearest"));
                    break;
                }
            case (VelocityComponentTypes.ROAD_MAP):
                {
                    if (m_StateNode.state.velocityComponent?.GetType() != typeof(RoadMapVelocityComponent))
                    {
                        m_StateNode.state.velocityComponent = new RoadMapVelocityComponent();
                        serializedObject.Update();
                    }
                    RoadMapVelocityComponent m_roadMapVelocityComponent = m_StateNode.state.velocityComponent as RoadMapVelocityComponent;

                    EditorGUILayout.LabelField("Road Map Velocity Component Parameters");

                    EditorGUILayout.BeginHorizontal();
                    m_roadMapVelocityComponent.fileName = EditorGUILayout.TextField("File Name", m_roadMapVelocityComponent.fileName);
                    if (GUILayout.Button("Select"))
                    {
                        m_roadMapVelocityComponent.fileName = Path.GetFileName(EditorUtility.OpenFilePanel("Road Map File", "", ""));
                        serializedObject.Update();
                    }
                    EditorGUILayout.EndHorizontal();
                    break;
                }
            case (VelocityComponentTypes.NAVIGATION_MESH):
                {
                    if (m_StateNode.state.velocityComponent?.GetType() != typeof(NavigationMeshVelocityComponent))
                    {
                        m_StateNode.state.velocityComponent = new NavigationMeshVelocityComponent();
                        serializedObject.Update();
                    }
                    NavigationMeshVelocityComponent m_navigationMeshVelocityComponent = m_StateNode.state.velocityComponent as NavigationMeshVelocityComponent;

                    EditorGUILayout.LabelField("Navigation Mesh Velocity Component Parameters");

                    EditorGUILayout.BeginHorizontal();
                    m_navigationMeshVelocityComponent.fileName = EditorGUILayout.TextField("File Name", m_navigationMeshVelocityComponent.fileName);
                    if (GUILayout.Button("Select"))
                    {
                        m_navigationMeshVelocityComponent.fileName = Path.GetFileName(EditorUtility.OpenFilePanel("Navigation Mesh File", "", "nav"));
                        serializedObject.Update();
                    }
                    EditorGUILayout.EndHorizontal();

                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("state.velocityComponent.headingThreshold"));
                    break;
                }
            default:
                break;
        }
    }

    private void showGoalSelectorPicker()
    {
        // Allows Adding a new action of the specified type
        m_StateNode.selectedGoalSelectorType = (GoalSelectorTypes)EditorGUILayout.EnumPopup("Goal Selector", m_StateNode.selectedGoalSelectorType);

        GoalSelector goalSelector = m_StateNode.state.goalSelector;
        switch (m_StateNode.selectedGoalSelectorType)
        {
            case GoalSelectorTypes.NONE:
                {
                    if (goalSelector != null) m_StateNode.state.goalSelector = null;
                    serializedObject.Update();
                    break;
                }
            case GoalSelectorTypes.IDENTITY_GOAL_SELECTOR:
                {
                    if (goalSelector?.GetType() != typeof(IdentityGoalSelector))
                    {
                        m_StateNode.state.goalSelector = new IdentityGoalSelector();
                        serializedObject.Update();
                        if (m_StateNode.HasPort("state.goalSelector.goal")) m_StateNode.RemoveDynamicPort("state.goalSelector.goal");
                    }
                    EditorGUILayout.LabelField("Identity Goal Selector");
                    break;
                }
            case GoalSelectorTypes.EXPLICIT_GOAL_SELECTOR:
                {
                    if (goalSelector?.GetType() != typeof(ExplicitGoalSelector))
                    {

                        m_StateNode.state.goalSelector = new ExplicitGoalSelector();
                        serializedObject.Update();
                        // NodePort np = new NodePort("state.goalSelector.goal", typeof(Goal), NodePort.IO.Input, Node.ConnectionType.Multiple, Node.TypeConstraint.None, m_StateNode);
                        if (m_StateNode.HasPort("state.goalSelector.goal")) m_StateNode.RemoveDynamicPort("state.goalSelector.goal");
                        m_StateNode.AddDynamicInput(typeof(Goal), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "state.goalSelector.goal");
                    }
                    NodeEditorGUILayout.PortField(new GUIContent("Selected Goal"), m_StateNode.GetPort("state.goalSelector.goal"));
                    break;
                }
            case GoalSelectorTypes.HEATMAP_GOAL_SELECTOR:
                {
                    if (goalSelector?.GetType() != typeof(HeatmapGoalSelector))
                    {
                        m_StateNode.state.goalSelector = new HeatmapGoalSelector();
                        serializedObject.Update();
                        // NodePort np = new NodePort("state.goalSelector.goal", typeof(Goal), NodePort.IO.Input, Node.ConnectionType.Multiple, Node.TypeConstraint.None, m_StateNode);
                        if (m_StateNode.HasPort("state.goalSelector")) m_StateNode.RemoveDynamicPort("state.goalSelector");
                        m_StateNode.AddDynamicInput(typeof(HeatmapGoalSelector), Node.ConnectionType.Override, Node.TypeConstraint.None, fieldName: "state.goalSelector");
                    }
                    NodeEditorGUILayout.PortField(new GUIContent("Heatmap Goal Selector"), m_StateNode.GetPort("state.goalSelector"));
                    break;
                }
            case GoalSelectorTypes.MIRROR_GOAL_SELECTOR:
                {
                    if (goalSelector?.GetType() != typeof(MirrorGoalSelector))
                    {
                        m_StateNode.state.goalSelector = new MirrorGoalSelector();
                        serializedObject.Update();
                        // NodePort np = new NodePort("state.goalSelector.goal", typeof(Goal), NodePort.IO.Input, Node.ConnectionType.Multiple, Node.TypeConstraint.None, m_StateNode);
                        if (m_StateNode.HasPort("state.goalSelector.goal")) m_StateNode.RemoveDynamicPort("state.goalSelector.goal");
                    }

                    EditorGUILayout.LabelField("Mirror Goal Selector Parameters");
                    EditorGUILayout.BeginHorizontal();
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("state.goalSelector.mirrorX"));
                    NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("state.goalSelector.mirrorY"));
                    EditorGUILayout.EndHorizontal();
                    break;
                }
            default:
                break;
        }
    }

    void OnCreateReorderableList(ReorderableList list)
    {

        // Override drawHeaderCallback to display node's name instead
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Actions");
        };

        // list.drawElementCallback += new ReorderableList.ElementCallbackDelegate(test);
        list.onAddCallback += new ReorderableList.AddCallbackDelegate(changeAddedActionToSelectedType);
    }

    // private void test(Rect rect, int index, bool isActive, bool isFocused)
    // {
    //     Debug.Log(index);
    // }

    // When the + button is pressed on the GUI list, it instantiates a new object of type Goal.
    // but we actually wamt an object of type "selectedAddGoalType", so we change the added item.
    void changeAddedActionToSelectedType(ReorderableList list)
    {
        m_StateNode.state.actions[m_StateNode.state.actions.Count - 1] = CreateAction(m_StateNode.selectedAddActionType);
    }

    public override int GetWidth()
    {
        return 300;
    }

    public override void OnHeaderGUI()
    {
        base.OnHeaderGUI();
    }

    public Action CreateAction(ActionTypes actionType)
    {
        Action action = null;
        switch (actionType)
        {
            case ActionTypes.SET_PROPERTY_ACTION:
                {
                    action = new SetPropertyAction();
                    break;
                }
            case ActionTypes.OFFSET_PROPERTY_ACTION:
                {
                    action = new OffsetPropertyAction();
                    break;
                }
            case ActionTypes.SCALE_PROPERTY_ACTION:
                {
                    action = new ScalePropertyAction();
                    break;
                }

            default:
                break;
        }
        return action;
    }

    public void AddAction(ActionTypes actionType)
    {
        switch (actionType)
        {
            case ActionTypes.SET_PROPERTY_ACTION:
                {
                    m_StateNode.state.actions.Add(CreateAction(actionType));
                    break;
                }
            case ActionTypes.OFFSET_PROPERTY_ACTION:
                {
                    m_StateNode.state.actions.Add(CreateAction(actionType));
                    break;
                }
            case ActionTypes.SCALE_PROPERTY_ACTION:
                {
                    m_StateNode.state.actions.Add(CreateAction(actionType));
                    break;
                }

            default:
                break;
        }
    }
}