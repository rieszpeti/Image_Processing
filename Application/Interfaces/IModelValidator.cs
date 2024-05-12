namespace Application.CSharp.Interfaces
{
    public interface IModelValidator
    {
        (bool, string?) Validate<T>(T entity);
    }
}