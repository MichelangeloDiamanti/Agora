using UnityEngine;
using UnityEditor;

public class DirectionGizmo : MonoBehaviour
{
    public enum Direction { Right, Left, Up, Down, Forward, Backward };

    public Direction direction = Direction.Forward;

    public float arrowHeadLength = 0.25f;
    public float arrowHeadAngle = 20.0f;
    public float lineThickness = 0.1f;

    public Color arrowColor = Color.green;

    private void OnDrawGizmosSelected()
    {
        Handles.color = arrowColor;

        Vector3 arrowDir = transform.forward;
        Debug.LogFormat("ArrowDir: [{0}, {1}, {2}]", arrowDir.x, arrowDir.y, arrowDir.z);

        switch (direction)
        {
            case Direction.Right:
                arrowDir = transform.right;
                break;
            case Direction.Left:
                arrowDir = -transform.right;
                break;
            case Direction.Up:
                arrowDir = transform.up;
                break;
            case Direction.Down:
                arrowDir = -transform.up;
                break;
            case Direction.Forward:
                arrowDir = transform.forward;
                break;
            case Direction.Backward:
                arrowDir = -transform.forward;
                break;
        }
        
        Vector3 arrowTip = transform.position + arrowDir;

        Handles.DrawLine(transform.position, arrowTip, lineThickness);

        Vector3 right = Quaternion.LookRotation(arrowDir) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(arrowDir) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Handles.DrawLine(arrowTip, arrowTip + right * arrowHeadLength, lineThickness);
        Handles.DrawLine(arrowTip, arrowTip + left * arrowHeadLength, lineThickness);
    }
}
