namespace ExpertStore.Ordering.Domain;

public class Person
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Address { get; private set; }

    public Person(int id, string name, string address)
    {
        Id = id;
        Name = name;
        Address = address;  
    }
}
