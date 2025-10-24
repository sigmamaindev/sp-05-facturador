namespace Core.DTOs;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public string Error { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
    public Pagination? Pagination { get; set; }
}

public class Pagination
{
    public int Total { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / Limit);
}
