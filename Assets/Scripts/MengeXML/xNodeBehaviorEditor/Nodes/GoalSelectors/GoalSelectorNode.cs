using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Menge.BFSM;

public abstract class GoalSelectorNode : Node
{
    public abstract Goal getGoal();
}
