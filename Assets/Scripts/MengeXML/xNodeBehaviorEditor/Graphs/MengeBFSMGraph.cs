using Menge.BFSM;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;
using XNode;

[CreateAssetMenu(menuName = "Menge/BFSM/BehavioralFiniteStateMachine")]

[XmlRoot(ElementName = "BFSM")]
public class MengeBFSMGraph : NodeGraph, IXmlSerializable
{
    public string XMLFilePath;

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        throw new System.NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {

        foreach (Node n in nodes)
        {
            if (n.GetType() == typeof(GoalSetNode))
            {
                GoalSetNode goalSetNode = n as GoalSetNode;
                XmlSerializer serializer = new XmlSerializer(typeof(GoalSet));
                serializer.Serialize(writer, goalSetNode.goalSet);
            }
            else if (n.GetType() == typeof(StateNode))
            {
                StateNode stateNode = n as StateNode;

                // The explicit goal selector wants to know the goalset id, so we update the value from the connection
                if (stateNode.state.goalSelector?.GetType() == typeof(ExplicitGoalSelector) &&
                    stateNode.HasPort("state.goalSelector.goal"))
                {
                    ExplicitGoalSelector egs = stateNode.state.goalSelector as ExplicitGoalSelector;
                    GoalNode gn = stateNode.GetPort("state.goalSelector.goal").Connection.node as GoalNode;
                    egs.goalSetId = gn.getGoalSet().id;
                }

                XmlSerializer serializer = new XmlSerializer(typeof(State));
                serializer.Serialize(writer, stateNode.state);
            }
            else if (n.GetType() == typeof(TransitionNode))
            {
                TransitionNode transitionNode = n as TransitionNode;
                XmlSerializer serializer = new XmlSerializer(typeof(Transition));
                serializer.Serialize(writer, transitionNode.transition);
            }
            else if (n.GetType() == typeof(VelocityModifierNode))
            {
                VelocityModifierNode velocityModifierNode = n as VelocityModifierNode;
                if (velocityModifierNode.GetOutputPort("velocityModifier").IsConnected)
                {
                    // if the node output is connected, it means that this velocity modifier
                    // is going to be serialized as part of a BFSM state, so no need to serialize it as a
                    // standalone element
                    continue;
                }
                else
                {
                    // if the node output is NOT connected, it means that this is a velocity modifier
                    // which applies to the whole BFSM, so it needs to be serialized as a standalone element
                    // XmlSerializer serializer = new XmlSerializer(typeof(VelocityModifier));
                    // serializer.Serialize(writer, velocityModifierNode.velocityModifier);
                    velocityModifierNode.velocityModifier.WriteXml(writer);
                }
            }
        }
    }

}