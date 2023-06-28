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
    public IActionResult GetTodos([FromServices] MySqlConnection connection, [FromQuery] bool? completed)
    {
        var sqlQuery = completed != null ? "SELECT * FROM todos WHERE completed=" + completed 
            : "SELECT * FROM todos";
        try
        {
            return Ok(connection.Query<Todo>(sqlQuery).ToList());
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpGet("{id}")]
    public Todo GetById(int id, [FromServices] MySqlConnection connection)
    {
        var sqlQuery = "SELECT * FROM todos WHERE id=" + id;
        return connection.QuerySingle<Todo>(sqlQuery);
    }

    [HttpPost("")]
    public IActionResult PostTodo([FromBody] TodoDto todoDto, [FromServices] MySqlConnection connection)
    {
        if (VerifyTodoDto(todoDto))
        {
            return BadRequest();
        }
        
        const string sqlQuery =
            "INSERT INTO todos (text, completed) VALUES (@Text, @Completed);" +
            "SELECT * FROM todos WHERE text=@Text AND completed=@Completed";

        var result = connection.QueryMultiple(sqlQuery, todoDto);

        var todo = result.Read<Todo>().ToList()[0];

        return Ok(todo);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Todo))]
    public IActionResult UpdateTodo([FromBody] TodoDto todoDto, [FromServices] MySqlConnection connection, string id)
    {
        if (VerifyTodoDto(todoDto))
        {
            return BadRequest();
        }
        
        var sqlQuery =
            "UPDATE todos SET completed=@Completed, text=@Text WHERE id=" + id + ";" +
            "SELECT * FROM todos WHERE text=@Text AND completed=@Completed";
        try
        {
            var result = connection.QueryMultiple(sqlQuery, todoDto);
            return Ok(result.Read<Todo>().ToList()[0]);
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

    private static bool VerifyTodoDto(TodoDto todoDto)
    {
        return todoDto.Text == null || todoDto.Completed == null || todoDto.Text == "";
    }
}