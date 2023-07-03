using System.ComponentModel.DataAnnotations;
using TestSolution.Models;
using TestSolution.Repositories;

namespace TestSolution.Services;

public class TodoService : ITodoService
{
    private readonly ILogger<TodoService> _logger;
    private readonly ITodoRepository _todoRepository;

    public TodoService(ILogger<TodoService> logger, ITodoRepository todoRepository)
    {
        _logger = logger;
        _todoRepository = todoRepository;
    }

    public IEnumerable<Todo> GetTodos(bool? completed)
    {
        return completed != null 
            ? _todoRepository.GetByCompleted((bool)completed) 
            : _todoRepository.GetAllTodos();
    }

    public Todo? GetTodoById(int id)
    {
        return _todoRepository.GetTodoById(id);
    }

    public Todo? PostTodo(TodoDto todoDto)
    {
        if (VerifyTodoDto(todoDto))
        {
            throw new ValidationException();
        }
        return _todoRepository.PostTodo(todoDto);
    }

    public Todo? UpdateTodo(TodoDto todoDto, int id)
    {
        return _todoRepository.UpdateTodo(todoDto, id);
    }

    public void DeleteTodo(int id)
    {
        _todoRepository.DeleteTodoById(id);
    }
    
    private static bool VerifyTodoDto(TodoDto todoDto)
    {
        return todoDto.Completed == null || string.IsNullOrWhiteSpace(todoDto.Text);
    }
}