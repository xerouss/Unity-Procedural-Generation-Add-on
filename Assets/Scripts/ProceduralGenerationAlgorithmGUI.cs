// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   23/04/2018
//	File:			   ProceduralGenerationAlgorithmGUI.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;
using UnityEditor;

////////////////////////////////////////////

/// <summary>
/// Bass class for the GUI classes for each algorithm
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class ProceduralGenerationAlgorithmGUI
    {
        #region Constants
        const int header1TextSize = 30;
        const int header2TextSize = 20;
        const int header3TextSize = 12;
        const int header4TextSize = 10;
        #endregion

        #region Variables
        string m_headerTitle;
        string m_tempSeed;
        GeneratorSeed m_seed;
        protected bool m_allowRealTimeGeneration = false;
        protected bool m_realTimeGenerationActive = false;
        protected string m_typeOfOutputtedLevel;

        #region Style Variables
        protected GUIStyle m_header2Style;
        protected GUIStyle m_header3Style;
        protected GUIStyle m_header4Style;
        #endregion
        #endregion

        #region Properties
        public bool RealTimeGenerationActive
        {
            get
            {
                return m_realTimeGenerationActive;
            }

            set
            {
                m_realTimeGenerationActive = value;
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ProceduralGenerationAlgorithmGUI(GeneratorSeed seed, string headerTitle,
            GUIStyle header2, GUIStyle header3, GUIStyle header4)
        {
            m_header2Style = header2;
            m_header3Style = header3;
            m_header4Style = header4;
            m_seed = seed;
            m_tempSeed = m_seed.Seed;
            m_headerTitle = headerTitle;
        }

        /// <summary>
        /// The GUI that will be displayed for the algorithm
        /// </summary>
        public virtual void DisplayGUI()
        {
            string fieldText;
            string tooltip;

            // Header
            GUILayout.Label(m_headerTitle, m_header2Style);
            EditorGUILayout.Space();

            // If the algorithm can have real-time generation, allow the user to switch it on/off
            if (m_allowRealTimeGeneration)
            {
                fieldText = "Real-time Generation: ";
                tooltip = "Update the terrain as you change it. Turn off for large levels as it may cause problems";
                m_realTimeGenerationActive = EditorGUILayout.Toggle(new GUIContent(fieldText, tooltip), m_realTimeGenerationActive);
            }

            // Seed            
            fieldText = "Seed: ";
            tooltip = "The seed of the current variables values";
            GUI.SetNextControlName("Seed Field"); // Set a name for the seed field to check if it's highlighted (look at the end of the function)
                                                  // Use a temp seed so the seed only changes when the user is not currently editing it/highlighted it
            m_tempSeed = EditorGUILayout.TextField(new GUIContent(fieldText, tooltip), m_tempSeed);

            // If the seed field is not selected
            if (GUI.GetNameOfFocusedControl() != "Seed Field")
            {
                // If the temp seed is different to the actual seed, make them the same
                // This is done when the field is not selected so if the user removes a number it does not produce an error
                // The if statement is required because there is no reason to change the seed if they are the same
                if (m_tempSeed != m_seed.Seed) m_seed.SetSeedToVariables(m_tempSeed);

                // Set the variable values to the seed values and set temp seed to the actual seed
                // This is done when the field is not selected because we don't want incorrect numbers appearing on the variables
                // When the user is moving around seed values
                m_tempSeed = m_seed.UpdateSeed();
            }

            EditorGUILayout.Space();

            // Rest will be filled in by the classes which inherit from this class
        }

        /// <summary>
        /// Spawn the level in the scene with a new game object
        /// </summary>
        public virtual void CreateLevel()
        {
            // Will be filled in by the classes which inherit from this class
        }

        /// <summary>
        /// Spawn the level in the scene using the game object for the level already in the scene
        /// </summary>
        public virtual void ReCreateLevel()
        {
            // Will be filled in by the classes which inherit from this class
        }

        /// <summary>
        /// Delete the current level from the scene
        /// </summary>
        public virtual void DeleteLevel()
        {
            // Will be filled in by the classes which inherit from this class
            // Tried making it so both the BSP and Perlin Noise use the same function
            // However, it would not get the generated level type properly from a string
            // And would just output null
        }

        /// <summary>
        /// Reset all the variables for the level generation to their defaults
        /// </summary>
        public virtual void ResetVariablesForLevel()
        {
            // Will be filled in by the classes which inherit from this class
        }
    }
}
