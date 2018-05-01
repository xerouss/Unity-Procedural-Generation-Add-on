// ***********************************************************************************
//	Name:	           Stephen Wong
//	Last Edited On:	   28/04/2018
//	File:			   ProceduralGenerationAlgorithm.cs
//	Project:		   Procedural Generation Add-on
// ***********************************************************************************

/// <summary>
/// This class is the base class for the procedural generation algorithms
/// It contains all functions which the algorithms share
/// These are mainly the functions for the properties
/// </summary>
namespace ProceduralGenerationAddOn
{
    public class ProceduralGenerationAlgorithm
    {
        /// <summary>
        /// Check if value is less than a number
        /// Used by the properties
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="minValue">The number to check if the value is less than</param>
        /// <returns>The checked value</returns>
        public float CheckIfValueIsLess(float value, int minValue)
        {
            if (value < minValue) value = minValue;
            return value;
        }

        /// <summary>
        /// Check if value is more than the passed number
        /// Used by the properties
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="maxValue">The number the value can't go over</param>
        /// <returns>The checked value</returns>
        public float CheckIfValueIsMore(float value, int maxValue)
        {
            if (value > maxValue) value = maxValue;
            return value;
        }

        /// <summary>
        /// Check if the value is in between two number
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <param name="minValue">The minimum number the value can be</param>
        /// <param name="maxValue">The maximum number the value can be</param>
        /// <returns>The value after the check</returns>
        public float CheckIfValueIsInbetween(float value, int minValue, int maxValue)
        {
            value = CheckIfValueIsLess(value, minValue);
            value = CheckIfValueIsMore(value, maxValue);

            return value;
        }

        /// <summary>
        /// Reset all variables the user can change back to their defaults
        /// </summary>
        public virtual void ResetVariableValues()
        {

        }
    }
}
