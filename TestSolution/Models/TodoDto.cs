namespace TestSolution.Models;

public class TodoDto
{
    private string _text;

    public TodoDto()
    {
    }

    public bool? Completed { get; set; }

    public int Id { get; set; }

    public string Text
    {
        get => _text;
        set => _text = value ?? throw new ArgumentNullException(nameof(value));
    }
}