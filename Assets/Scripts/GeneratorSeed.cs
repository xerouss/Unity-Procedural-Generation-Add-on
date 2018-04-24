// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   24/04/2018
//	File:			   GeneratorSeed.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

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
		const int removeLengthInt = 1;
		#endregion

		#region Variables
		string m_seedValue = "0"; // Seed has to be a string because the length is too big for an int
		protected int m_numOfVariablesUserCanChange;
		#endregion

		#region Properties
		public string SeedValue
		{
			get
			{
				return m_seedValue;
			}

			set
			{
				m_seedValue = value;
			}
		}
		#endregion

		/// <summary>
		/// Set the passed seed to the variables for the generation
		/// </summary>
		/// <param name="newSeedValue">The new seed</param>
		public void SetSeedToVariables(string newSeedValue)
		{
			// Don't need to change the variables if they are the same
			if (m_seedValue == newSeedValue) return;

			int numCheckValue = 0;
			bool numCheck;

			// Go through seed and check the characters in it are all valid
			for (int i = 0; i < newSeedValue.Length; i++)
			{
				// Check if it's a number
				numCheck = int.TryParse(newSeedValue[i].ToString(), out numCheckValue);

				// If it isn't a number check if it is a - for negative number or . for decimal points
				// If it isn't one of them there is an incorrect character so don't set the variable to the seed
				if (!numCheck)
				{
					if (newSeedValue[i] != '-' && newSeedValue[i] != '.') return;
				}
			}

			// -1 the length because it starts at base 0
			int indexLower = newSeedValue.Length - makeBase0;
			int indexUpper = newSeedValue.Length - makeBase0;
			string numberString = "";
			float numberFloat = 0;
			string valueString = newSeedValue;

			// ************************************************************************************
			// The seeds work by having a number which tells the class the size of the value to get
			// For example: 1123, the 3 tells the class the next 3 numbers are for the variable
			// So the variable will be set to 112
			// ************************************************************************************

			// Go through all variables the user can change set the values to the correct variables
			// -1 the num of variables that can be changed because the last value is at 0 and not 1
			for (int i = m_numOfVariablesUserCanChange - makeBase0; i >= 0; i--)
			{
				// Get the first int of the seed
				// This is used to find the length of the variable value
				indexLower -= (int)char.GetNumericValue(valueString[indexLower]);

				// Get the variable value by adding each int in the string together
				// Goes from left to right so the number is in the correct order
				// -1 from the upper so it does not get the in used to get the length of the variable value
				for (int j = indexLower; j <= indexUpper - removeLengthInt; j++)
				{
					numberString += valueString[j];
				}

				// Change it to an int and set it to the correct variable
				numberFloat = float.Parse(numberString);
				SetUserVariable(i, numberFloat);

				// -1 from lower to move to the next variable's length
				// The upper is now the lower value since we are now on the next variable
				indexLower -= goToNextVariable;
				indexUpper = indexLower;

				// Reset the number since we don't want to keep adding to the previous variable
				numberString = "";
			}

            // Set new seed
			m_seedValue = newSeedValue;
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
				// Convert the variable to string so the length of it can be added to the seed
				variableValueString = GetUserVariable(i).ToString();
				variableValueString += variableValueString.Length.ToString();

				// Add it to the total seed value
				seedValue += variableValueString;
			}

			// Save the new seed
			m_seedValue = seedValue;

            // Need to ouput the seed for the tempSeed in the editor window
			return m_seedValue;
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
