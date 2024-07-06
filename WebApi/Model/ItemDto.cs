namespace WebApi.Model;

public class Item
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public DateTime CreatedAt { get; set; }
	public int Version { get; set; }
}
