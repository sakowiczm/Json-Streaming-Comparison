using Models;

public class DataSource
{
    public IEnumerable<Person> GetPeople()
    {
        var firstNames = new[] { "John", "Jane", "Alice", "Bob", "Charlie", "Diana", "Eve", "Frank" };
        var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis" };
        var random = new Random();
        while (true)
        {
            yield return new Person
            {
                FirstName = firstNames[random.Next(firstNames.Length)],
                LastName = lastNames[random.Next(lastNames.Length)],
                Age = random.Next(1, 100)
            };
        }
    }
}
