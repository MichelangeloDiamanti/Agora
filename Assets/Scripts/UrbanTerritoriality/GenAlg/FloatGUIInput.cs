using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UrbanTerritoriality.GenAlg
{

    /** An input field with a label
     * for a floating point number. This input field
     * can be displayed on screen by calling
     * the CreateGUI method inside an OnGUI method
     * in an class extending MonoBehavior.
     */
    public class FloatGUIInput
    {
        /** The value stored in the float input */
        private float value;

        /** The actual string in the field */
        private string fieldValue;

        /** The label of the field */
        private string text;

        /** Becomes true if converting the string
         * in the field to a float number fails. */
        private bool error = false;

        /** The height of the input field */
        public static int Height { get { return 20;} }

        /** The width of the label next to the input field. */
        public static int LabelWidth { get { return 150; } }

        /** The width of the input field */
        public static int InputWidth { get { return 50; } }

        /** The distance between the label and the input field. */
        public static int Padding { get { return 10; } }

        /** The Width of the entire GUI component. */
        public static int Width
        { get { return LabelWidth + Padding + InputWidth; } }

        /** The number in the input field */
        public float Value
        {
            get { return value; }
            set
            {
                this.value = value;
                this.fieldValue = this.value.ToString();
            }
        }

        /** The text in the label */
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /** Constructor
         * @param value The value of the field.
         * @param text The label text.
         */
        public FloatGUIInput(float value, string text)
        {
            this.value = value;
            this.fieldValue = value.ToString();
            this.text = text;
        }

        /**
         * Create the GUI components and process input.
         * Call this inside the OnGUI method in a class
         * extending MonoBehavior.
         * @param x The x value of the upper left corner of the
         * component.
         * @param y The y value of the upper left corner of the
         * component.
         */ 
        public void CreateGUI(int x, int y)
        {
            GUI.color = error ? new Color(1, 0, 0, 1) :
                new Color(0, 1, 0, 1);
            GUI.Label(new Rect(x, y, LabelWidth, Height), text);
            fieldValue = GUI.TextField(new Rect(LabelWidth + Padding,
                y, InputWidth, Height), fieldValue, 25);
            if (GUI.changed)
            {
                try
                {
                    float val = (float)System.Convert.ToDouble(fieldValue);
                    value = val;
                    error = false;
                }
                catch { error = true; }
            }
        }
    }
}

