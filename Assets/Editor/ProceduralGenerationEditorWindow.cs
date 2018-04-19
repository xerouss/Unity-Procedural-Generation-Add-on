// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   19/04/2018
//	File:			   ProceduralGenerationEditorWindow.cs
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

        #region Constants
        const int header1TextSize = 30;
        const int header2TextSize = 20;
        const int header3TextSize = 12;
        const int minWindowSize = 480;
        const int dropdownSpaceHeight = 15;
        const int heightmapResLowerBound = 0;
        const int heightmapResHigherBound = 512;
        #endregion

        #region Private
        enum LevelTypes
        {
            TERRAIN, DUNGEON, NUMOFLEVELTYPES
        }

        int m_levelType = 0;
        bool m_realtimeGeneration = false;
        string[] m_levelTypeOptions = { "Terrain", "Dungeon" };
        static string m_tempSeed;
        static PerlinNoise m_perlinNoise;
        static BinarySpacePartition m_binarySpacePartition;

        #region Style Variables
        static GUIStyle m_header1Style;
        static GUIStyle m_header2Style;
        static GUIStyle m_header3Style;
        #endregion
        #endregion

        #endregion

        // Where to access the window
        [MenuItem("Window/Procedurally Generate Level")]

        /// <summary>
        /// How to display the window
        /// </summary>
        public static void ShowWindow()
        {
            // Create the editor window
            EditorWindow procGenWindow = EditorWindow.GetWindow(typeof(ProceduralGenerationEditorWindow));
            // The min size the window can be
            // This is so the window shows the header text when it is created
            procGenWindow.minSize = new Vector2(minWindowSize, minWindowSize);

            // Create the styles for the window
            CreateStyles();

            // Create the perlin noise so it can be used
            m_perlinNoise = new PerlinNoise(256 * 2);

            m_tempSeed = m_perlinNoise.Seed;

            m_binarySpacePartition = new BinarySpacePartition();
        }

        /// <summary>
        /// Create all the styles used in the GUI
        /// </summary>
        static void CreateStyles()
        {
            m_header1Style = new GUIStyle();
            m_header1Style.fontSize = header1TextSize;
            m_header1Style.fontStyle = FontStyle.Bold;

            m_header2Style = new GUIStyle();
            m_header2Style.fontSize = header2TextSize;
            m_header2Style.fontStyle = FontStyle.Bold;

            m_header3Style = new GUIStyle();
            m_header3Style.fontSize = header3TextSize;
            m_header3Style.fontStyle = FontStyle.Bold;
        }

        private void Update()
        {
            if(m_perlinNoise != null && m_realtimeGeneration) m_perlinNoise.SetTerrainData();
        }

        /// <summary>
        /// What to display in the window
        /// </summary>
        void OnGUI()
        {
            // Header
            GUILayout.Label("Procedurally Generate Level", m_header1Style);
            EditorGUILayout.Space();

            GUILayout.Label("Hover over the variable name for information on what they change.");
            EditorGUILayout.Space();

            // Dropdown box to select the level 
            m_levelType = EditorGUILayout.Popup("Level Type:", m_levelType, m_levelTypeOptions);
            GUILayout.Space(dropdownSpaceHeight);

            // Based on the level selected, select the correct level type variables
            switch (m_levelType)
            {
                case (int)LevelTypes.TERRAIN:
                    TerrainGUI();
                    break;
                case (int)LevelTypes.DUNGEON:
                    DungeonGUI();
                    break;
                case (int)LevelTypes.NUMOFLEVELTYPES:
                default:
                    Debug.Log("ERROR: UNKOWN LEVEL TYPE");
                    break;
            }

            // If the user wants to update the terrain as they change it
            m_realtimeGeneration = EditorGUILayout.Toggle(new GUIContent("Real-time Generation: ", "Update the terrain as you change it. Turn off for large terrains as it may cause problems"), m_realtimeGeneration);

            // Don't need the button if the terrain is being auto updated
            if (!m_realtimeGeneration)
            {

                // Create a new GameObject that contains the level
                // TODO: Implement this, does not create a new game object at the moment
                // STILL NEED WITH THE REALTIME UPDATING?
                if (GUILayout.Button("Create Level"))
                {
                    if (m_levelType == (int)LevelTypes.TERRAIN)
                    {
                        m_perlinNoise.SetTerrainData();
                    }
                    else
                    {
                        m_binarySpacePartition.CreateCells();
                        m_binarySpacePartition.CreateRooms();
                        m_binarySpacePartition.CreateCorridors();
                    }
                }

                // Create the level using the existing GameObject
                if (GUILayout.Button("Re-create Level"))
                {
                    if (m_levelType == (int)LevelTypes.TERRAIN)
                    {
                        m_perlinNoise.SetTerrainData();
                    }
                }

                // Delete the level's GameObject
                if (GUILayout.Button("Delete Level"))
                {
                    DestroyImmediate(GameObject.FindObjectOfType<Terrain>().gameObject);
                }
            }

            // Reset all variables to the default values
            if (GUILayout.Button("Reset Variables"))
            {
                if (m_levelType == (int)LevelTypes.TERRAIN)
                {
                    m_perlinNoise.ResetVariableValues();
                }
            }
        }

        /// <summary>
        /// Variables for the terrain generation
        /// </summary>
        void TerrainGUI()
        {
            // TODO: Improve descriptions

            // Header
            GUILayout.Label("Terrain", m_header2Style);
            EditorGUILayout.Space();

            // Seed
            GUI.SetNextControlName("Seed Field"); // Set a name for the seed field to check if it's highlighted (look at the end of the function)
            // Use a temp seed so the seed only changes when the user is not currently editing it/highlighted it
            m_tempSeed = EditorGUILayout.TextField(new GUIContent("Seed: ", "The seed of the current variables values"), m_tempSeed);

            // Terrain size
            m_perlinNoise.TerrainSize = EditorGUILayout.Vector3Field(new GUIContent("Terrain Size:", "X = Width, Y = Higher the spikes in the level are, Z = Depth") , m_perlinNoise.TerrainSize);
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

            // If the seed field is not selected
            if(GUI.GetNameOfFocusedControl() != "Seed Field")
            {
                // If the temp seed if different to the actual seed, make them the same
                // This is done when the field is not selected so if the user removes a number it does not produce an error
                // The if statement is required because the is no reason to change the seed if they are the same
                if (m_tempSeed != m_perlinNoise.Seed) m_perlinNoise.SetSeedToVariables(m_tempSeed);

                // Set the variable values to the seed values and set temp seed to the actual see
                // This is done when the field is not selected because we don't want incorrect numbers appearing on the variables
                // When the user is moving around seed values
                m_tempSeed = m_perlinNoise.UpdateSeed();
            }
        }

        /// <summary>
        /// Variables for the dungeon generation
        /// </summary>
        void DungeonGUI()
        {
            GUILayout.Label("Dungeon", m_header2Style);
            EditorGUILayout.Space();

            m_binarySpacePartition.FloorTile = EditorGUILayout.ObjectField(new GUIContent("Floor Tile: ", "The tile used for the floor of the dungeon"), m_binarySpacePartition.FloorTile, typeof(GameObject), false) as GameObject;

            ///////////////////////////////
            //TODO REMOVE THIS AFTER DEBUGGING
            EditorGUILayout.Space();

            m_binarySpacePartition.FloorTile2 = EditorGUILayout.ObjectField(new GUIContent("Floor Tile: ", "The tile used for the floor of the dungeon"), m_binarySpacePartition.FloorTile2, typeof(GameObject), false) as GameObject;
            EditorGUILayout.Space();

            m_binarySpacePartition.FloorTile3 = EditorGUILayout.ObjectField(new GUIContent("Floor Tile: ", "The tile used for the floor of the dungeon"), m_binarySpacePartition.FloorTile3, typeof(GameObject), false) as GameObject;
            ///////////////////////////////
        }
    }
}
