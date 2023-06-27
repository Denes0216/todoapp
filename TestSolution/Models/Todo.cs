namespace TestSolution.Models;

public class Todo
{
    private string _text;

    public Todo(int id, bool completed, string text)
    {
        Completed = completed;
        Id = id;
        _text = text;
    }

    public Todo(bool completed, string text)
    {
        Completed = completed;
        Id = 0;
        _text = text;
    }

    public bool Completed { get; set; }

    public int Id { get; set; }

    public string Text
    {
        get => _text;
        set => _text = value ?? throw new ArgumentNullException(nameof(value));
    }
}