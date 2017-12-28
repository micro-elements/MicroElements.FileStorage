namespace MicroElements.FileStorage.Tests.Models
{
    /// <summary>
    /// Sample person model.
    /// </summary>
    public class Person
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(FirstName)}: {FirstName}, {nameof(LastName)}: {LastName}";
        }
    }
}