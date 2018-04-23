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

            // Terrain size
            m_perlinNoise.TerrainSize = EditorGUILayout.Vector3Field(new GUIContent("Terrain Size:", "X = Width, Y = Higher the spikes in the level are, Z = Depth"), m_perlinNoise.TerrainSize);
            EditorGUILayout.Space();

            // Heightmap Resolution
            m_perlinNoise.HeightmapResolution = EditorGUILayout.IntSlider(new GUIContent("Heightmap Resolution: ", "Default = 32"),
                m_perlinNoise.HeightmapResolution, heightmapResLowerBound, heightmapResHigherBound);
            EditorGUILayout.Space();

            // Offset
            m_perlinNoise.PosOffset = EditorGUILayout.Vector2Field(new GUIContent("Position Offset: ", "Change the position of the perlin noise"), m_perlinNoise.PosOffset);
            EditorGUILayout.Space();

            //// Fade Change
            GUILayout.Label("Fade Change", m_header3Style);
            m_perlinNoise.MultiplyFade = EditorGUILayout.IntField(new GUIContent("Multiply Value: ", "Default = 6"), m_perlinNoise.MultiplyFade);
            m_perlinNoise.MinusFade = EditorGUILayout.IntField(new GUIContent("Minus Value: ", "Default = 15"), m_perlinNoise.MinusFade);
            m_perlinNoise.AdditionFade = EditorGUILayout.IntField(new GUIContent("Addition Value: ", "Default = 10"), m_perlinNoise.AdditionFade);
            EditorGUILayout.Space();

            // Fractal
            GUILayout.Label("Fractal Brownian Motion", m_header3Style);
            m_perlinNoise.Octaves = EditorGUILayout.FloatField(new GUIContent("Octaves: ", "Amount of times it iterates. More = more detail"), m_perlinNoise.Octaves);
            m_perlinNoise.Frequency = EditorGUILayout.FloatField(new GUIContent("Frequency: ", "How much the bumps are spread out. Lower = flatter"), m_perlinNoise.Frequency);
            m_perlinNoise.Amplitude = EditorGUILayout.FloatField(new GUIContent("Amplitude: ", "How flat/tall it is"), m_perlinNoise.Amplitude);
            m_perlinNoise.AmplitudeGain = EditorGUILayout.FloatField(new GUIContent("Amplitude Gain: ", "How much the amplitude increases after each iteration"), m_perlinNoise.AmplitudeGain);
            m_perlinNoise.Lacunarity = EditorGUILayout.FloatField(new GUIContent("Lacunarity: ", "How much the frequency is increased after each iteration"), m_perlinNoise.Lacunarity);
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Create the terrain from the perlin noise
        /// </summary>
        public override void CreateLevel()
        {
            m_perlinNoise.SetTerrainData();
        }

        /// <summary>
        /// Re-use an already made terrain for a new level layout
        /// </summary>
        public override void ReCreateLevel()
        {
            m_perlinNoise.SetTerrainData();
        }

        /// <summary>
        /// Delete the currently used terrain
        /// </summary>
        public override void DeleteLevel()
        {
           MonoBehaviour.DestroyImmediate(GameObject.FindObjectOfType<Terrain>().gameObject);
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
