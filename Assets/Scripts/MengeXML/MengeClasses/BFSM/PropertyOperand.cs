using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Menge
{
    namespace BFSM
    {
        public enum PropertyOperand
        {
            no_property,    ///< The "NULL" property - indicating no valid property.
            max_speed,      ///< The agent's maximum speed.
            max_accel,      ///< The agent's maximum, isotropic acceleration.
            pref_speed,     ///< The agent's preferred speed.
            max_angle_vel,  ///< The agent's maximum angular velocity
            neighbor_dist,  ///< The agent's neighbor distance
            priority,       ///< The agent's priority
            radius          ///< The agent's radius
        }
    }
}