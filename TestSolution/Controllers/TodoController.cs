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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Todo))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetTodos([FromServices] MySqlConnection connection, [FromQuery] bool? completed)
    {
        _logger.Log(LogLevel.Information, "Get todos was called");
        var sqlQuery = completed != null
            ? "SELECT * FROM todos WHERE completed=" + completed
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

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Todo))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id, [FromServices] MySqlConnection connection)
    {
        var sqlQuery = "SELECT * FROM todos WHERE id=" + id;
        try
        {
            return Ok(connection.QuerySingle<Todo>(sqlQuery));
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Todo))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult PostTodo([FromBody] TodoDto todoDto, [FromServices] MySqlConnection connection)
    {
        if (VerifyTodoDto(todoDto))
        {
            return BadRequest();
        }
        _logger.Log(LogLevel.Information, "Post was called", todoDto);
        
        const string sqlQuery =
            "INSERT INTO todos (text, completed) VALUES (@Text, @Completed);" +
            "SELECT * FROM todos WHERE text=@Text AND completed=@Completed";

        var result = connection.QueryMultiple(sqlQuery, todoDto);

        return Ok(result.ReadFirst<Todo>());
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Todo))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdateTodo([FromBody] TodoDto todoDto, [FromServices] MySqlConnection connection, int id)
    {
        _logger.Log(LogLevel.Information, "Put was called", todoDto);
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
            return Ok(result.ReadFirst<Todo>());
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Todo))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult DeleteTodo([FromServices] MySqlConnection connection, int id)
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
    }

    private static bool VerifyTodoDto(TodoDto todoDto)
    {
        return todoDto.Completed == null || string.IsNullOrWhiteSpace(todoDto.Text);
    }
}