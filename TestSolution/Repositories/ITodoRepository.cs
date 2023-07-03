using TestSolution.Models;

namespace TestSolution.Repositories;

public interface ITodoRepository
{
    IEnumerable<Todo> GetAllTodos();

    IEnumerable<Todo> GetByCompleted(bool completed);

    Todo? GetTodoById(int id);

    Todo? PostTodo(TodoDto todoDto);

    Todo? UpdateTodo(TodoDto todoDto, int id);
 
    void DeleteTodoById(int id);
}