namespace SaleGz.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"'{name}' con id '{key}' no fue encontrado.") { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException() : base("No tienes permisos para realizar esta acción.") { }
}

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Se encontraron uno o más errores de validación.")
    {
        Errors = errors;
    }
}

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}
