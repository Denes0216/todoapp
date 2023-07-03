using TestSolution.Models;

namespace TestSolution.Services;

public interface ITodoService
{
    IEnumerable<Todo> GetTodos(bool? completed);

    Todo? GetTodoById(int id);

    Todo? PostTodo(TodoDto todoDto);

    Todo? UpdateTodo(TodoDto todoDto, int id);

    void DeleteTodo(int id);
}