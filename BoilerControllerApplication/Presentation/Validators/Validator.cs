namespace BoilerControllerApplication.Presentation.Validators
{
    public static class Validator
    {
        /// <summary>
        /// Checks if the input is valid integer
        /// </summary>
        /// <param name="value">The string value user entered</param>
        /// <param name="userChoice">The integer value of the user input</param>
        /// <returns>true if able to parse string to integer, otherwise false</returns>
        public static bool IsValidInteger(string value, out int userChoice)
        {
            return int.TryParse(value, out userChoice);
        }
    }
}
