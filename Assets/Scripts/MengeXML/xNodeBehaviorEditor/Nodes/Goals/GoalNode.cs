using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;

public abstract class GoalNode : Node
{
    public abstract Goal getGoal();

    public abstract GoalSet getGoalSet();
}
