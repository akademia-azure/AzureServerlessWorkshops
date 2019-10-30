using Microsoft.WindowsAzure.Storage.Table;

namespace Onboarding.Models
{
    public class OnboardingDataEntity : TableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Office { get; set; }
        public string Position { get; set; }
    }
}
