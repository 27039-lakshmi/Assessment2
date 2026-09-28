namespace BoilerControllerApplication.Presentation.Validators
{
    public static class Validator
    {
        public static bool IsValidInteger(string value, out int userChoice)
        {
            return int.TryParse(value, out userChoice);
        }
    }
}
