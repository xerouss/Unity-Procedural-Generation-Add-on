// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   22/04/2018
//	File:			   GeneratorSeed.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

////////////////////////////////////////////
// Libraries and namespaces
using UnityEngine;

////////////////////////////////////////////

/// <summary>
/// This class is responsible for the seed that is used for the generation.
/// This is the base class which handles the functionality of getting/setting the seed.
/// The child classes will handles the functionality of getting/setting the variable values
/// </summary>
namespace ProceduralGenerationAddOn
{
	public class GeneratorSeed
	{
		#region Constants
		const int makeBase0 = 1;
		const int goToNextVariable = 1;
		#endregion

		#region Variables
		string m_seed = "0";
        protected int m_numOfVariablesUserCanChange;
        #endregion

        #region Properties
        public string Seed
		{
			get
			{
				return m_seed;
			}

			set
			{
				m_seed = value;
			}
		}
		#endregion

		/// <summary>
		/// Set the passed seed to the variables for the generation
		/// </summary>
		/// <param name="newSeed">The new seed</param>
		public void SetSeedToVariables(string newSeed)
		{
			// Don't need to change the variables if they are the same
			// Make sure the length is correct (is num of variable *2 because of the length num)
			if (m_seed == newSeed) return;

			int numCheckValue = 0;
			bool numCheck;

			// Go through seed
			for (int i = 0; i < newSeed.Length; i++)
			{
				// Check if it's a number
				numCheck = int.TryParse(newSeed[i].ToString(), out numCheckValue);

				// If it isn't a number check if it is a - for negative number or . for decimal points
				// If it isn't one of them there is an incorrect character so don't set the variable to the seed
				if (!numCheck)
				{
					if (newSeed[i] != '-' && newSeed[i] != '.') return;
				}
			}

			// -1 the length because it starts at base 0
			int indexLower = newSeed.Length - makeBase0;
			int indexUpper = newSeed.Length - makeBase0;
			string numberString = "";
			float numberFloat = 0;

			string valueString = newSeed;

			// Go through all variables the user can change
            // TODO why -1???
			for (int i = m_numOfVariablesUserCanChange - 1; i >= 0; i--)
			{
				// Find where the variable value gets cut off
				indexLower -= (int)char.GetNumericValue(valueString[indexLower]);

				// Get the variable value
				for (int j = indexLower; j <= indexUpper - 1; j++)
				{
					numberString += valueString[j];
				}

				// Change it to an int and set it to the correct variables
				numberFloat = float.Parse(numberString);
				SetUserVariable(i, numberFloat);

				// -1 from lower to move to the next variable's length
				// The upper is now the lower value since we are now on the next variable
				indexLower -= goToNextVariable;
				indexUpper = indexLower;

				// Reset the number since we don't want to keep adding to the previous variable
				numberString = "";
			}

			m_seed = newSeed;
		}

		/// <summary>
		/// Updates the seed value with the current variable inputs
		/// </summary>
		/// <returns>The seed value</returns> 
		public string UpdateSeed()
		{
			string seedValue = "";
			string variableValueString = "";

			// Get all variable values
			for (int i = 0; i < m_numOfVariablesUserCanChange; i++)
			{
				// Convert the variable to string so the length of it can be added to the end of it
				variableValueString = GetUserVariable(i).ToString();
				variableValueString += variableValueString.Length.ToString();

				// Add it to the total seed value
				seedValue += variableValueString;
			}

			// Convert the value to an int and set it to the seed
			m_seed = seedValue;

			return m_seed;
		}

		/// <summary>
		/// Get the value of one of the variables that the user can change
		/// </summary>
		/// <param name="index">The variable to get the value from</param>
		/// <returns>Variable value</returns>
		public virtual float GetUserVariable(int index)
		{
            // Will be filled in by the classes which inherit from this class
            return 0;
		}

		/// <summary>
		/// Set a value to a variable that the user can change
		/// </summary>
		/// <param name="index">The variable to set the value to</param>
		/// <param name="value">The new value</param>
		public virtual void SetUserVariable(int index, float value)
		{
            // Will be filled in by the classes which inherit from this class
        }
    }
}
