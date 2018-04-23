// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   23/04/2018
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
        const int heightmapResLowerBound = 0;
        const int heightmapResHigherBound = 512;
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
            // TODO: Improve descriptions
            base.DisplayGUI();

            string fieldText;
            string tooltip;

            // Terrain size
            fieldText = "Terrain Size:";
            tooltip = "X = Width, Y = Higher the spikes in the level are, Z = Depth";
            m_perlinNoise.TerrainSize = EditorGUILayout.Vector3Field(new GUIContent(fieldText, tooltip), m_perlinNoise.TerrainSize);
            EditorGUILayout.Space();

            // Heightmap Resolution
            fieldText = "Heightmap Resolution: ";
            tooltip = "Default = 32";
            m_perlinNoise.HeightmapResolution = EditorGUILayout.IntSlider(new GUIContent(fieldText, tooltip),
                m_perlinNoise.HeightmapResolution, heightmapResLowerBound, heightmapResHigherBound);
            EditorGUILayout.Space();

            // Offset
            fieldText = "Position Offset: ";
            tooltip = "Change the position of the perlin noise";
            m_perlinNoise.PosOffset = EditorGUILayout.Vector2Field(new GUIContent(fieldText, tooltip), m_perlinNoise.PosOffset);
            EditorGUILayout.Space();

            // *************************************************
            //// Fade Change
            GUILayout.Label("Fade Change", m_header3Style);

            fieldText = "Multiply Value: ";
            tooltip = "Default = 6";
            m_perlinNoise.MultiplyFade = EditorGUILayout.IntField(new GUIContent(fieldText, tooltip), m_perlinNoise.MultiplyFade);

            fieldText = "Minus Value: ";
            tooltip = "Default = 15";
            m_perlinNoise.MinusFade = EditorGUILayout.IntField(new GUIContent(fieldText, tooltip), m_perlinNoise.MinusFade);

            fieldText = "Addition Value: ";
            tooltip = "Default = 10";
            m_perlinNoise.AdditionFade = EditorGUILayout.IntField(new GUIContent(fieldText, tooltip), m_perlinNoise.AdditionFade);
            EditorGUILayout.Space();
            // *************************************************

            // *************************************************
            // Fractal
            GUILayout.Label("Fractal Brownian Motion", m_header3Style);

            fieldText = "Octaves: ";
            tooltip = "Amount of times it iterates. More = more detail";
            m_perlinNoise.Octaves = EditorGUILayout.FloatField(new GUIContent(fieldText, tooltip), m_perlinNoise.Octaves);

            fieldText = "Frequency: ";
            tooltip = "How much the bumps are spread out. Lower = flatter";
            m_perlinNoise.Frequency = EditorGUILayout.FloatField(new GUIContent(fieldText, tooltip), m_perlinNoise.Frequency);

            fieldText = "Amplitude: ";
            tooltip = "How flat/tall it is";
            m_perlinNoise.Amplitude = EditorGUILayout.FloatField(new GUIContent(fieldText, tooltip), m_perlinNoise.Amplitude);

            fieldText = "Amplitude Gain: ";
            tooltip = "How much the amplitude increases after each iteration";
            m_perlinNoise.AmplitudeGain = EditorGUILayout.FloatField(new GUIContent(fieldText, tooltip), m_perlinNoise.AmplitudeGain);

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
            MonoBehaviour.DestroyImmediate(GameObject.FindObjectOfType<GeneratedTerrain>().gameObject);
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
