// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   27/02/2018
//	File:			   Procedural Generation Editor Window.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;
using UnityEditor;

////////////////////////////////////////////

/// <summary>
/// This is responsible for the window which holds everything in the add-on
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class ProceduralGenerationEditorWindow : EditorWindow
    {

        #region Variables

        #region Private

        static PerlinNoise m_perlinNoise;

        #endregion

        #endregion

        // Where to access the window
        [MenuItem("Window/Procedural Generate Level")]

        /// <summary>
        /// How to display the window
        /// </summary>
        public static void ShowWindow()
        {
            m_perlinNoise = new PerlinNoise();
            EditorWindow.GetWindow(typeof(ProceduralGenerationEditorWindow));
        }

        /// <summary>
        /// What to display in the window
        /// </summary>
        void OnGUI()
        {
            // Temp, here to test the level creation
            if(GUI.Button(new Rect(10, 10, 150, 50), "Generate Level"))
            {
                m_perlinNoise.CreateGrid(new Vector2(10, 10));
                m_perlinNoise.GetTerrain();
            }
        }
    }
}
