using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Menge
{
    namespace BFSM
    {

        [CustomEditor(typeof(FormationScriptableObject))]
        public class FormationScriptableObjectEditor : Editor
        {
            public float zoom = 1.0f;
            public string filePath;

            public ReorderableList formationList;

            private FormationScriptableObject formation;

            private int selectedFormationPointIndex;
            private Vector2 rightClickPosition;

            Rect formationDrawerRect;

            float formationDrawerScale = 10f;
            float formationDrawerAgentRadius = 5f;

            Color formationDrawerBackgroundColor = new Color(0.165f, 0.165f, 0.165f);
            Color formationDrawerAgentColor = new Color(0.502f, 0.702f, 1f);
            Color formationDrawerAgentColorSelected = new Color(1f, 1f, 1f);
            Color formationDrawerCenterColor = new Color(1f, 0.4f, 0.4f);


            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                DrawFormationDrawer();
                HandleDragFormationPoint();

                zoom = EditorGUILayout.Slider("Zoom", zoom, 0.1f, 10f);

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Save Formation"))
                {
                    filePath = EditorUtility.OpenFilePanel("Formation File", "", "txt");

                    if (filePath != "")
                    {
                        formation.WriteFormationFile(filePath);
                        Debug.LogFormat("Saved formation to: {0}", filePath);
                    }
                }

                GUILayout.EndHorizontal();


                formationList.DoLayoutList();

                EditorUtility.SetDirty(formation);

                // Apply property modifications
                serializedObject.ApplyModifiedProperties();
            }

            private void HandleDragFormationPoint()
            {
                //Debug.Log(EditorWindow.focusedWindow.titleContent);
                //if(Event.current.type == EventType.MouseDown)
                //Debug.Log(Event.current);
                switch (Event.current.type)
                {
                    case (EventType.MouseDown):
                        {
                            // Left Click
                            if (Event.current.button == 0)
                            {
                                Vector2 clickPosition = Event.current.mousePosition;
                                if (formationDrawerRect.Contains(clickPosition))
                                {
                                    Vector3 clickInRectSpace = new Vector3(-(formationDrawerRect.center.x - clickPosition.x), formationDrawerRect.center.y - clickPosition.y, 0);

                                    //Debug.Log("You Clicked Here: " + clickInRectSpace);

                                    for (int i = 0; i < formation.formationPoints.Count; i++)
                                    {
                                        Vector2 fPoint = formation.formationPoints[i].position;

                                        Vector2 fPointPosition = fPoint * formationDrawerScale * zoom;

                                        if (inCircle(clickInRectSpace, fPointPosition, formationDrawerAgentRadius))
                                        {
                                            //Debug.Log("You Selected Point: " + i);
                                            selectedFormationPointIndex = i;
                                            formationList.index = i;
                                            Repaint();
                                            break;
                                        }
                                    }

                                }
                            }

                            // Right Click
                            else if (Event.current.button == 1 && formationDrawerRect.Contains(Event.current.mousePosition))
                            {
                                Vector2 clickInRectSpace = new Vector2(-(formationDrawerRect.center.x - Event.current.mousePosition.x), formationDrawerRect.center.y - Event.current.mousePosition.y);
                                clickInRectSpace = clickInRectSpace / formationDrawerScale / zoom;
                                rightClickPosition = clickInRectSpace;

                                GenericMenu menu = new GenericMenu();
                                menu.AddItem(new GUIContent("Add Formation Point"), false, AddFormationPoint, null);
                                menu.ShowAsContext();
                            }

                            break;
                        }
                    case (EventType.MouseUp):
                        {
                            selectedFormationPointIndex = -1;
                            break;
                        }
                    case (EventType.MouseDrag):
                        {
                            if (selectedFormationPointIndex != -1)
                            {
                                Vector2 mousePositionInRectSpace = new Vector2(-(formationDrawerRect.center.x - Event.current.mousePosition.x), formationDrawerRect.center.y - Event.current.mousePosition.y);
                                mousePositionInRectSpace = mousePositionInRectSpace / formationDrawerScale / zoom;

                                formation.formationPoints[selectedFormationPointIndex].position = new Vector2(mousePositionInRectSpace.x, mousePositionInRectSpace.y);
                            }
                            break;
                        }
                    default:
                        break;
                }

            }

            private void AddFormationPoint(object obj)
            {
                formation.formationPoints.Add(new FormationPoint()
                {
                    position = rightClickPosition,
                    border = false,
                    weight = 1f
                });
            }

            private bool inCircle(Vector2 point, Vector2 circleCenter, float circleRadius)
            {
                float dx = Mathf.Abs(point.x - circleCenter.x);
                if (dx > circleRadius) return false;
                float dy = Mathf.Abs(point.y - circleCenter.y);
                if (dy > circleRadius) return false;
                if (dx + dy <= circleRadius) return true;
                return (dx * dx + dy * dy <= circleRadius * circleRadius);
            }

            private void DrawFormationDrawer()
            {
                Vector2 rectSize = new Vector2(300, 300);

                Rect parentRect = GUILayoutUtility.GetRect(rectSize.x, rectSize.y);
                formationDrawerRect = new Rect(new Vector2(parentRect.center.x - (rectSize.x / 2), parentRect.position.y), rectSize);

                EditorGUI.DrawRect(formationDrawerRect, formationDrawerBackgroundColor);

                for (int i = 0; i < formation.formationPoints.Count; i++)
                {
                    Vector2 fPoint = formation.formationPoints[i].position;

                    Vector3 fPointPosition = new Vector3(formationDrawerRect.center.x + fPoint.x * formationDrawerScale * zoom, formationDrawerRect.center.y - fPoint.y * formationDrawerScale * zoom, 0);
                    if (formationDrawerRect.Contains(fPointPosition))
                    {
                        Handles.color = formationDrawerAgentColor;
                        if (i == formationList.index)
                        {
                            Handles.color = formationDrawerAgentColorSelected;
                        }
                        Handles.DrawSolidDisc(fPointPosition, new Vector3(0, 0, 1), formationDrawerAgentRadius);
                        Handles.color = Color.black;
                        Handles.DrawWireDisc(fPointPosition, new Vector3(0, 0, 1), formationDrawerAgentRadius);
                    }
                }

                Handles.color = formationDrawerCenterColor;
                Handles.DrawSolidDisc(new Vector3(formationDrawerRect.center.x, formationDrawerRect.center.y, 0), new Vector3(0, 0, 1), formationDrawerAgentRadius);
                Handles.color = Color.black;
                Handles.DrawWireDisc(new Vector3(formationDrawerRect.center.x, formationDrawerRect.center.y, 0), new Vector3(0, 0, 1), formationDrawerAgentRadius);

            }

            private void OnEnable()
            {
                formation = (FormationScriptableObject)target;

                formationList = new ReorderableList(formation.formationPoints, typeof(FormationPoint));
                formationList.drawHeaderCallback += DrawHeader;
                formationList.drawElementCallback += DrawElement;
            }

            public void OnDisable()
            {
                formationList.drawHeaderCallback -= DrawHeader;
                formationList.drawElementCallback -= DrawElement;
            }

            protected virtual void DrawHeader(Rect rect)
            {
                int elements = 5;

                float xPadding = 10f;
                float xOffset = 0f;
                Rect nameRect = new Rect(rect.position, new Vector2(rect.width / elements, rect.height));
                xOffset += nameRect.width + xPadding;
                Rect posXRect = new Rect(rect.position.x + xOffset, rect.position.y, rect.width / elements, rect.height);
                xOffset += posXRect.width + xPadding;
                Rect posYRect = new Rect(rect.position.x + xOffset, rect.position.y, rect.width / elements, rect.height);
                xOffset += posYRect.width + xPadding;
                Rect weightRect = new Rect(rect.position.x + xOffset, rect.position.y, rect.width / elements, rect.height);
                xOffset += weightRect.width + xPadding;
                Rect borderRect = new Rect(rect.position.x + xOffset, rect.position.y, rect.width / elements, rect.height);


                GUI.Label(nameRect, "Name");
                GUI.Label(posXRect, "Pos X");
                GUI.Label(posYRect, "Pos Y");
                GUI.Label(weightRect, "Weight");
                GUI.Label(borderRect, "Border");
            }

            protected void DrawElement(Rect rect, int index, bool active, bool focused)
            {
                FormationPoint currentItem = formation.formationPoints[index];

                if (currentItem != null)
                {
                    EditorGUI.BeginChangeCheck();

                    int elements = 5;

                    float xPadding = 10f;
                    float heightBorder = 4f;
                    float xOffset = 0f;

                    Rect nameRect = new Rect(rect.position.x + xOffset, rect.position.y, rect.width / elements, rect.height);
                    xOffset += nameRect.width + xPadding;
                    Rect posXRect = new Rect(rect.position.x + xOffset, rect.position.y + heightBorder / 2, rect.width / elements, rect.height - heightBorder);
                    xOffset += posXRect.width + xPadding;
                    Rect posYRect = new Rect(rect.position.x + xOffset, rect.position.y + heightBorder / 2, rect.width / elements, rect.height - heightBorder);
                    xOffset += posYRect.width + xPadding;
                    Rect weightRect = new Rect(rect.position.x + xOffset, rect.position.y + heightBorder / 2, rect.width / elements, rect.height - heightBorder);
                    xOffset += weightRect.width + xPadding;
                    Rect borderRect = new Rect(rect.position.x + xOffset, rect.position.y + heightBorder / 2, rect.width / elements, rect.height - heightBorder);


                    GUI.Label(nameRect, string.Format("Point {0}", index));
                    currentItem.position.x = EditorGUI.FloatField(posXRect, currentItem.position.x);
                    currentItem.position.y = EditorGUI.FloatField(posYRect, currentItem.position.y);
                    currentItem.weight = EditorGUI.FloatField(weightRect, currentItem.weight);
                    currentItem.border = EditorGUI.Toggle(borderRect, currentItem.border);

                    EditorGUI.EndChangeCheck();
                }
            }

        }
    }
}