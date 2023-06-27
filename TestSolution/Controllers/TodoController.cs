using Dapper;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using TestSolution.Models;

namespace TestSolution.Controllers;

[ApiController]
[Route("/todos")]
public class TodoController : Controller
{
    private readonly ILogger<TodoController> _logger;

    public TodoController(ILogger<TodoController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public List<Todo> GetTodos([FromServices] MySqlConnection connection)
    {
        const string sqlQuery = "SELECT * FROM todos";
        var resultList = connection.Query<Todo>(sqlQuery).ToList();
        return resultList;
    }

    [HttpGet("/todos/{id}")]
    public Todo GetById(int id, [FromServices] MySqlConnection connection)
    {
        var sqlQuery = "SELECT * FROM todos WHERE id=" + id;
        var result = connection.QuerySingle(sqlQuery);
        return result;
    }

    [HttpPost("")]
    public OkObjectResult PostTodo([FromBody] TodoDto todoDto, [FromServices] MySqlConnection connection)
    {
        const string sqlQuery =
            "INSERT INTO todos (text, completed) VALUES (@Text, @Completed);" +
            "SELECT * FROM todos WHERE text=@Text AND completed=@Completed";

        var result = connection.QueryMultiple(sqlQuery, todoDto);

        var todo = result.Read<Todo>().ToList()[0];

        return Ok(todo);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Todo))]
    public OkObjectResult UpdateTodo([FromBody] TodoDto todoDto, [FromServices] MySqlConnection connection, string id)
    {
        var sqlQuery =
            "UPDATE todos SET completed=@Completed, text=@Text WHERE id=" + id + ";" +
            "SELECT * FROM todos WHERE text=@Text AND completed=@Completed";

        var result = connection.QueryMultiple(sqlQuery, todoDto);

        var todo = result.Read<Todo>().ToList()[0];

        return Ok(todo);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Todo))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult DeleteTodo([FromServices] MySqlConnection connection, string id)
    {
        var sqlQuery =
            "SELECT * FROM todos WHERE id=" + id + "; " +
            "DELETE FROM todos WHERE id=" + id;
        try
        {
            var result = connection.QueryMultiple(sqlQuery);
            var todo = result.Read<Todo>().ToList()[0];
            return Ok(todo);
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
        catch (MySqlException e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }
}