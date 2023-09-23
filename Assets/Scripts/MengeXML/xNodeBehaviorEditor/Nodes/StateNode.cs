using Menge.BFSM;
using UnityEngine;
using XNode;

public class StateNode : Node
{
    [SerializeReference] public State state;

    [Input] Transition inTransition;

    [Output] State outState;

    public ActionTypes selectedAddActionType;

    public GoalSelectorTypes selectedGoalSelectorType;

    public VelocityComponentTypes selectedVelocityComponentType;

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "outState")
        {
            return state;
        }
        return null;
    }

    protected override void Init()
    {
        if (state == null) state = new State();
        base.Init();
    }

    public override void OnCreateConnection(NodePort from, NodePort to)
    {
        if (to.fieldName == "state.goalSelector.goal")
        {
            if (state.goalSelector.GetType() == typeof(ExplicitGoalSelector))
            {
                ExplicitGoalSelector gs = state.goalSelector as ExplicitGoalSelector;
                gs.goal = from.GetOutputValue() as Goal;
                state.goalSelector = gs;
            }
        }
        else if (to.fieldName == "state.velocityModifier")
        {
            state.velocityModifier = from.GetOutputValue() as VelocityModifier;
        }
        base.OnCreateConnection(from, to);
    }

    public override void OnRemoveConnection(NodePort port)
    {
        if (port.fieldName == "state.goalSelector.goal")
        {
            if (state.goalSelector.GetType() == typeof(ExplicitGoalSelector))
            {
                ExplicitGoalSelector gs = state.goalSelector as ExplicitGoalSelector;
                gs.goal = null;
                state.goalSelector = gs;
            }
        }
        else if (port.fieldName == "state.velocityModifier")
        {
            state.velocityModifier = null;
        }

        base.OnRemoveConnection(port);
    }
}