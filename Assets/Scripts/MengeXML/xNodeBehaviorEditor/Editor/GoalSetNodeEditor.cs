using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using XNode;
using XNodeEditor;
using Menge.BFSM;

[CustomNodeEditor(typeof(GoalSetNode))]
public class GoalSetNodeEditor : NodeEditor
{
    private GoalSetNode m_GoalSetNode;

    private GoalTypes selectedAddGoalType;

    public override void OnBodyGUI()
    {
        if (m_GoalSetNode == null) m_GoalSetNode = target as GoalSetNode;

        // SerializedObject serializedObject = new SerializedObject(m_GoalSetNode);
        serializedObject.Update();


        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("goalSet.id"), false);


        // Goalset Nodeport
        if (!m_GoalSetNode.HasPort("outputGoalSet")) m_GoalSetNode.AddDynamicOutput(typeof(GoalSet), Node.ConnectionType.Multiple, Node.TypeConstraint.None, fieldName: "outputGoalSet");
        NodePort outputGoalSet = m_GoalSetNode.GetPort("outputGoalSet");
        NodeEditorGUILayout.PortField(new GUIContent("Goal Set"), outputGoalSet);

        // // Allows Adding a new goal of the specified type
        // selectedAddGoalType = (GoalTypes)EditorGUILayout.EnumPopup("Goal to Add", selectedAddGoalType);

        // // Draw GUI
        // NodeEditorGUILayout.DynamicPortList(
        //     "goalSet.goals", // field name
        //     typeof(Goal), // field type
        //     serializedObject, // serializable object
        //     NodePort.IO.Output, // new port i/o
        //     Node.ConnectionType.Override, // new port connection type
        //     Node.TypeConstraint.None,
        //     OnCreateReorderableList); // onCreate override. This is where the magic happens.

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }

    void OnCreateReorderableList(ReorderableList list)
    {

        // Override drawHeaderCallback to display node's name instead
        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Goals");
        };

        list.onAddCallback += new ReorderableList.AddCallbackDelegate(changeAddedGoalToSelectedType);
    }

    // When the + button is pressed on the GUI list, it instantiates a new object of type Goal.
    // but we actually wamt an object of type "selectedAddGoalType", so we change the added item.
    void changeAddedGoalToSelectedType(ReorderableList list)
    {
        m_GoalSetNode.goalSet.goals[m_GoalSetNode.goalSet.goals.Count - 1] = CreateGoal(selectedAddGoalType);
    }

    public override int GetWidth()
    {
        return 200;
    }

    public Goal CreateGoal(GoalTypes goalType)
    {
        Goal goal = null;
        switch (goalType)
        {
            case GoalTypes.POINT_GOAL:
                {
                    goal = new PointGoal()
                    {
                        id = m_GoalSetNode.goalSet.goals.Count + 1,
                        capacity = int.MaxValue,
                        weight = 1.0f,
                        x = 0,
                        y = 0
                    };
                    break;
                }
            case GoalTypes.CIRCLE_GOAL:
                {
                    goal = new CircleGoal()
                    {
                        id = m_GoalSetNode.goalSet.goals.Count + 1,
                        capacity = int.MaxValue,
                        weight = 1.0f,
                        x = 0,
                        y = 0,
                        radius = 0f
                    };
                    break;
                }
            case GoalTypes.AXIS_ALIGNED_BOUNDING_BOX:
                {
                    goal = new AABBGoal()
                    {
                        id = m_GoalSetNode.goalSet.goals.Count + 1,
                        capacity = int.MaxValue,
                        weight = 1.0f,
                        min_x = 0f,
                        max_x = 0f,
                        min_y = 0f,
                        max_y = 0f
                    };
                    break;
                }
            case GoalTypes.ORIENTED_BOUNDING_BOX:
                {
                    goal = new OBBGoal()
                    {
                        id = m_GoalSetNode.goalSet.goals.Count + 1,
                        capacity = int.MaxValue,
                        weight = 1.0f,
                        x = 0f,
                        y = 0f,
                        width = 0f,
                        height = 0f,
                        angle = 0f
                    };
                    break;
                }
            case GoalTypes.HEATMAP_GOAL:
                {
                    goal = new AbsoluteHeatmapGoal()
                    {
                        id = m_GoalSetNode.goalSet.goals.Count + 1,
                        capacity = int.MaxValue,
                        weight = 1.0f
                    };
                    break;
                }

            default:
                break;
        }
        return goal;
    }

    public void AddGoal(GoalTypes goalType)
    {
        m_GoalSetNode.goalSet.goals.Add(CreateGoal(goalType));
        // switch (goalType)
        // {
        //     case GoalTypes.POINT_GOAL:
        //         {
        //             m_GoalSetNode.goalSet.goals.Add(CreateGoal(goalType));
        //             break;
        //         }
        //     case GoalTypes.CIRCLE_GOAL:
        //         {
        //             m_GoalSetNode.goalSet.goals.Add(CreateGoal(goalType));
        //             break;
        //         }
        //     case GoalTypes.AXIS_ALIGNED_BOUNDING_BOX:
        //         {
        //             m_GoalSetNode.goalSet.goals.Add(CreateGoal(goalType));
        //             break;
        //         }
        //     case GoalTypes.ORIENTED_BOUNDING_BOX:
        //         {
        //             m_GoalSetNode.goalSet.goals.Add(CreateGoal(goalType));
        //             break;
        //         }
        //     case GoalTypes.HEATMAP_GOAL:
        //         {
        //             m_GoalSetNode.goalSet.goals.Add(CreateGoal(goalType));
        //             break;
        //         }
        //     default:
        //         break;
        // }
    }
}