// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   29/04/2018
//	File:			   PerlinNoiseGUI.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;
using UnityEditor;

////////////////////////////////////////////

/// <summary>
/// GUI Class for the Perlin Noise
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class PerlinNoiseGUI : ProceduralGenerationAlgorithmGUI
    {
        #region Constants
        const string title = "Terrain";
        const bool dontCreateNewTerrain = false;
        const bool createNewTerrain = true;
        #endregion

        #region Variables
        PerlinNoise m_perlinNoise;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PerlinNoiseGUI(GeneratorSeed seed,  PerlinNoise perlin,
            GUIStyle header2, GUIStyle header3, GUIStyle header4) :
            base(seed, title, header2,  header3,  header4)
        {
            // Call the ProceduralGenerationAlgorithmGUI constructor

            m_perlinNoise = perlin;
            m_allowRealTimeGeneration = true;
        }

        /// <summary>
        /// The GUI that for Perlin Noise
        /// </summary>
        public override void DisplayGUI()
        {
            base.DisplayGUI(); // Display the base class's GUI

            string fieldText;
            string tooltip;

            // Terrain size
            fieldText = "Terrain Size:";
            tooltip = "X = Width, Y = Higher the terrain is moved up, Z = Depth";
            m_perlinNoise.TerrainSize = EditorGUILayout.Vector3Field(new GUIContent(fieldText, tooltip), m_perlinNoise.TerrainSize);
            EditorGUILayout.Space();

            // Heightmap Resolution
            // The heightmap resolution has upper and lower bounds to prevent extreme values
            // High values may caused the editor to crash and negative values errors
            fieldText = "Heightmap Resolution: ";
            tooltip = "How detailed the terrain would be output. The more detailed, the clearer the bends are in the terrain. Higher = more detail. Default = 128";
            m_perlinNoise.HeightmapResolution = EditorGUILayout.IntSlider(new GUIContent(fieldText, tooltip),
                m_perlinNoise.HeightmapResolution, PerlinNoise.heightmapResLowerBound, PerlinNoise.heightmapResHigherBound);
            EditorGUILayout.Space();

            // Offset
            fieldText = "Position Offset: ";
            tooltip = "Change the position of the perlin noise so different layouts of terrain can be accessed";
            m_perlinNoise.PosOffset = EditorGUILayout.Vector2Field(new GUIContent(fieldText, tooltip), m_perlinNoise.PosOffset);
            EditorGUILayout.Space();

            // *************************************************
            //// Fade Change
            GUILayout.Label("Fade Change", m_header3Style);

            // Multiply Value
            fieldText = "Multiply Value: ";
            tooltip = "Change the multiply value in the fade function. This will change how the terrain is smoothed. Large values, even negative ones, can lead to pointy/blocky features. Default = 6";
            m_perlinNoise.MultiplyFade = EditorGUILayout.IntField(new GUIContent(fieldText, tooltip), m_perlinNoise.MultiplyFade);

            // Minus Value
            fieldText = "Minus Value: ";
            tooltip = "Change the minus value in the fade function. This will change how the terrain is smoothed. Large values, even negative ones, can lead to pointy/blocky features. Default = 15";
            m_perlinNoise.MinusFade = EditorGUILayout.IntField(new GUIContent(fieldText, tooltip), m_perlinNoise.MinusFade);

            // Addition Value
            fieldText = "Addition Value: ";
            tooltip = "Change the addition value in the fade function. This will change how the terrain is smoothed. Large values, even negative ones, can lead to pointy/blocky features. Default = 10";
            m_perlinNoise.AdditionFade = EditorGUILayout.IntField(new GUIContent(fieldText, tooltip), m_perlinNoise.AdditionFade);
            EditorGUILayout.Space();
            // *************************************************

            // *************************************************
            // Fractal
            GUILayout.Label("Fractal Brownian Motion", m_header3Style);
            if(m_realTimeGenerationActive) GUILayout.Label("When using a high octave amount, turn off real-time generation.", m_header4Style);

            // Octaves
            fieldText = "Octaves: ";
            tooltip = "Amount of times it iterates. More = more detail. May need to turn down frequency as this increases. Turn off real-time generation on high values as it can slow down the editor.";
            m_perlinNoise.Octaves = EditorGUILayout.FloatField(new GUIContent(fieldText, tooltip), m_perlinNoise.Octaves);

            // Frequency
            fieldText = "Frequency: ";
            tooltip = "How much the bumps are spread out. Lower = flatter";
            m_perlinNoise.Frequency = EditorGUILayout.FloatField(new GUIContent(fieldText, tooltip), m_perlinNoise.Frequency);

            // Amplitude
            fieldText = "Amplitude: ";
            tooltip = "How flat/tall it is";
            m_perlinNoise.Amplitude = EditorGUILayout.FloatField(new GUIContent(fieldText, tooltip), m_perlinNoise.Amplitude);

            // Amplitude Gain
            fieldText = "Amplitude Gain: ";
            tooltip = "How much the amplitude increases after each iteration. Only works if octaves is higher than 1";
            m_perlinNoise.AmplitudeGain = EditorGUILayout.FloatField(new GUIContent(fieldText, tooltip), m_perlinNoise.AmplitudeGain);

            // Lacunarity
            fieldText = "Lacunarity: ";
            tooltip = "How much the frequency is increased after each iteration";
            m_perlinNoise.Lacunarity = EditorGUILayout.FloatField(new GUIContent(fieldText, tooltip), m_perlinNoise.Lacunarity);
            EditorGUILayout.Space();
            // *************************************************
        }

        /// <summary>
        /// Create the terrain from the perlin noise
        /// </summary>
        public override void CreateLevel()
        {
            m_perlinNoise.SetTerrainData(createNewTerrain);
        }

        /// <summary>
        /// Re-use an already made terrain for a new level layout
        /// </summary>
        public override void ReCreateLevel()
        {
            m_perlinNoise.SetTerrainData(dontCreateNewTerrain);
        }

        /// <summary>
        /// Delete the current level from the scene
        /// </summary>
        public override void DeleteLevel()
        {
            // Check if there is something to delete first or else there will be an error
            GeneratedTerrain outputtedTerrain = GameObject.FindObjectOfType<GeneratedTerrain>();

            if(outputtedTerrain != null)
                MonoBehaviour.DestroyImmediate(outputtedTerrain.gameObject);
        }

        /// <summary>
        /// Reset the variables for the Perlin Noise back to the default
        /// </summary>
        public override void ResetVariablesForLevel()
        {
            m_perlinNoise.ResetVariableValues();
        }
    }
}
