// using UnityEditor;
// using UnityEditor.UIElements;
// using UnityEngine.UIElements;
// using UnityEngine;
// using Menge.Math;
// using Menge.BFSM;

// // IngredientDrawerUIE
// [CustomPropertyDrawer(typeof(FloatGenerator))]
// public class RandGeneratorDrawer : PropertyDrawer
// {
//     GeneratorTypes selectedGeneratorType;

//     // Draw the property inside the given rect
//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {

//         // Allows Adding a new goal of the specified type
//         selectedGeneratorType = (GeneratorTypes)EditorGUI.EnumPopup(position, "Generator Type", selectedGeneratorType);


//         // if (property != null)
//         // {
//         //     switch (selectedGeneratorType)
//         //     {
//         //         case (GeneratorTypes.CONSTANT_FLOAT):
//         //             {
//         //                 Rect valueRect = new Rect(position.x, position.y + 50, position.width, position.height);
//         //                 EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);
//         //                 break;
//         //             }
//         //         default:
//         //             break;
//         //     }
//         // }

//     }
// }