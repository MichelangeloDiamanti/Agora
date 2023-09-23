using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Menge
{
    namespace BFSM
    {

        [CreateAssetMenu(fileName = "Formation Velocity Modifier", menuName = "Menge/VelModifiers/Formation", order = 1)]
        public class FormationScriptableObject : ScriptableObject
        {
            public List<FormationPoint> formationPoints;

            public FormationScriptableObject()
            {
                this.formationPoints = new List<FormationPoint>();
            }

            public void WriteFormationFile(string filePath)
            {
                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    int borderPoints = 0;
                    foreach (FormationPoint fp in formationPoints)
                    {
                        if (fp.border) borderPoints++;
                    }
                    sw.WriteLine(borderPoints);
                    foreach (FormationPoint fp in formationPoints)
                    {
                        if (fp.border)
                        {
                            sw.Write(fp.position.x.ToString(nfi));
                            sw.Write(" ");
                            sw.Write(fp.position.y.ToString(nfi));
                            sw.Write(" ");
                            sw.Write(fp.weight.ToString(nfi));
                            sw.Write("\n");
                        }
                    }
                    foreach (FormationPoint fp in formationPoints)
                    {
                        if (!fp.border)
                        {
                            sw.Write(fp.position.x.ToString(nfi));
                            sw.Write(" ");
                            sw.Write(fp.position.y.ToString(nfi));
                            sw.Write(" ");
                            sw.Write(fp.weight.ToString(nfi));
                            sw.Write("\n");
                        }
                    }
                }
            }

        }

        [Serializable]
        public class FormationPoint
        {
            public Vector2 position;
            public float weight;
            public bool border;
        }

        // IngredientDrawer
        [CustomPropertyDrawer(typeof(FormationPoint))]
        public class FormationPointDrawer : PropertyDrawer
        {
            // Draw the property inside the given rect
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                // Using BeginProperty / EndProperty on the parent property means that
                // prefab override logic works on the entire property.
                EditorGUI.BeginProperty(position, label, property);

                // Draw label
                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                // Don't make child fields be indented
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                // Calculate rects
                float padding = 10f;
                float offset = 0f;

                var positionRect = new Rect(position.x, position.y, 200, position.height);
                offset += positionRect.width + padding;
                var weightLabelRect = new Rect(position.x + offset, position.y, 50, position.height);
                offset += weightLabelRect.width + padding;
                var weightRect = new Rect(position.x + offset, position.y, 50, position.height);
                offset += weightRect.width + padding;
                var borderLableRect = new Rect(position.x + offset, position.y, 50, position.height);
                offset += borderLableRect.width + padding;
                var borderRect = new Rect(position.x + offset, position.y, 50, position.height);

                // Draw fields - pass GUIContent.none to each so they are drawn without labels
                EditorGUI.PropertyField(positionRect, property.FindPropertyRelative("position"), GUIContent.none);
                EditorGUI.LabelField(weightLabelRect, "Weight");
                EditorGUI.PropertyField(weightRect, property.FindPropertyRelative("weight"), GUIContent.none);
                EditorGUI.LabelField(borderLableRect, "border");
                EditorGUI.PropertyField(borderRect, property.FindPropertyRelative("border"), GUIContent.none);

                // Set indent back to what it was
                EditorGUI.indentLevel = indent;

                EditorGUI.EndProperty();
            }
        }

    }
}