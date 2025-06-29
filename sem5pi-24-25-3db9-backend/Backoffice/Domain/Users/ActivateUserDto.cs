namespace Backoffice.Domain.Users
{
    public class ActivateUserDto
    {
        public Guid Id { get; set; }

        public string Passwd { get; set; }

        public bool Active { get; set; }
    }
}