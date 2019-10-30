namespace OnboardingWithEventGrid.Models
{
    public class OnboardingInputData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Office { get; set; }
        public string Position { get; set; }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(FirstName) ||
                string.IsNullOrEmpty(LastName) ||
                string.IsNullOrEmpty(Email) ||
                string.IsNullOrEmpty(Office) ||
                string.IsNullOrEmpty(Position))
                return false;

            return true;
        }
    }
}
