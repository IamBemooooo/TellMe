namespace TellMe.Core.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public List<string> Errors { get; set; } = new();
        public int? Total { get; set; }
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public int? TotalPages { get; set; }

        public static Result<T> Success(T data)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Data = data
            };
        }

        public static Result<T> SuccessResult(T data, string? message = null)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Data = data,
                SuccessMessage = message
            };
        }

        public static Result<T> Failure(string errorMessage)
        {
            return new Result<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        public static Result<T> Failure(List<string> errors)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Errors = errors
            };
        }

        public static Result<T> PaginatedResult(T data, int total, int page, int limit, string? message = null)
        {
            return new Result<T>
            {
                IsSuccess = true,
                Data = data,
                Total = total,
                Page = page,
                Limit = limit,
                TotalPages = (int)Math.Ceiling((double)total / limit),
                SuccessMessage = message
            };
        }
    }

    public class ResultWithPaging<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public string? ErrorMessage { get; set; }
    }
}
