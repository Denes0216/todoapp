using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TestSolution.Models;
using TestSolution.Services;

namespace TestSolution.Controllers;

[ApiController]
[Route("/todos")]
public class TodoController : Controller
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Todo))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult GetTodos([FromQuery] bool? completed)
    {
        try
        {
            return Ok(_todoService.GetTodos(completed));
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
    public IActionResult GetById(int id)
    {
        try
        {
            return Ok(_todoService.GetTodoById(id));
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
            return NotFound();
        }
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Todo))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult PostTodo([FromBody] TodoDto todoDto)
    {
        try
        {
            return Ok(_todoService.PostTodo(todoDto));
        }
        catch (ValidationException e)
        {
            return BadRequest(e);
        }
        
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Todo))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult UpdateTodo([FromBody] TodoDto todoDto, int id)
    {
        try
        {
            return Ok(_todoService.UpdateTodo(todoDto, id));
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
    public IActionResult DeleteTodo(int id)
    {
        try
        {
            _todoService.DeleteTodo(id);
            return Ok();
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine(e);
            return BadRequest();
        }
    }
}