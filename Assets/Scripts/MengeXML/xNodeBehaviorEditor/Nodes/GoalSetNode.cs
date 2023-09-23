using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

public class GoalSetNode : Node, IXmlSerializable
{
    [SerializeReference] public GoalSet goalSet;

    public override object GetValue(NodePort port)
    {
        if (port.fieldName.Contains("goalSet.goals"))
        {

            string strGoalIndex = port.fieldName.Replace("goalSet.goals ", "");
            int goalIndex = int.Parse(strGoalIndex);
            return goalSet.goals[goalIndex];
        }
        else if(port.fieldName == "outputGoalSet")
        {
            updateGoalset();
            return goalSet;
        }
        else return null;
    }

    public void updateGoalset()
    {
        goalSet.goals.Clear();
        NodePort goalsetPort = GetPort("outputGoalSet");
        if (goalsetPort != null && goalsetPort.IsConnected)
        {
            int goalIndex = 0;
            foreach (NodePort port in goalsetPort.GetConnections())
            {
                GoalNode goalNode = port.node as GoalNode;
                if(goalNode != null)
                {
                    goalNode.getGoal().id = goalIndex;
                    goalSet.goals.Add(goalNode.getGoal());
                }
                goalIndex++;
            }
        }
    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    public void WriteXml(XmlWriter writer)
    {
        XmlSerializer serializer = new XmlSerializer(goalSet.GetType());
        serializer.Serialize(writer, goalSet);
    }

    protected override void Init()
    {
        if (goalSet == null) goalSet = new GoalSet();
        base.Init();
    }
}