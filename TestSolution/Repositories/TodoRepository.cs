using Dapper;
using TestSolution.Models;
using TestSolution.Services;

namespace TestSolution.Repositories;

public class TodoRepository : ITodoRepository
{
    
    private readonly IDbService _dbService;

    public TodoRepository(IDbService dbService)
    {
        _dbService = dbService;
    }
    public IEnumerable<Todo> GetAllTodos()
    {
        using var db = _dbService.CreateConnection();
        return db.Query<Todo>("SELECT * FROM todos");
    }

    public IEnumerable<Todo> GetByCompleted(bool completed)
    {
        using var db = _dbService.CreateConnection();
        return db.Query<Todo>("SELECT * FROM todos WHERE completed=" + completed);
    }

    public Todo? GetTodoById(int id)
    {
        using var db = _dbService.CreateConnection();
        return db.QuerySingleOrDefault<Todo>("SELECT * FROM todos WHERE id=" + id);
    }

    public Todo? PostTodo(TodoDto todoDto)
    {
        using var db = _dbService.CreateConnection();
        const string sqlQuery =
            "INSERT INTO todos (text, completed) VALUES (@Text, @Completed);" +
            "SELECT * FROM todos WHERE text=@Text AND completed=@Completed";

        return db.QuerySingle<Todo>(sqlQuery, todoDto);
    }

    public Todo? UpdateTodo(TodoDto todoDto, int id)
    {
        using var db = _dbService.CreateConnection();
        var sqlQuery = "UPDATE todos SET text=@text,completed=@completed WHERE id=" + id+";SELECT * FROM todos WHERE id=" + id;
        return db.QuerySingle<Todo>(sqlQuery, todoDto);
    }

    public void DeleteTodoById(int id)
    {
        using var db = _dbService.CreateConnection();
        var sqlQuery = "DELETE FROM todos WHERE id=" + id;
        db.Query(sqlQuery);
    }
}