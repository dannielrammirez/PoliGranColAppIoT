namespace PoliGranColAppIoT.Models
{
    public class Account
    {
        public string username { get; set; }
        public string password { get; set; }

        public bool IsValid()
        {
            bool userNameValid = !string.IsNullOrEmpty(username);
            bool passwordValid = !string.IsNullOrEmpty(password);

            return userNameValid && passwordValid;
        }
    }
}