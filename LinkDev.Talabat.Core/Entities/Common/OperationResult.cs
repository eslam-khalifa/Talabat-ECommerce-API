namespace LinkDev.Talabat.Core.Entities.Common
{
    public class OperationResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public IReadOnlyList<string> Errors { get; private set; }

        private OperationResult(bool isSuccess, T? data = default, IReadOnlyList<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            Errors = errors ?? new List<string>();
        }

        public static OperationResult<T> Success() => new OperationResult<T>(true);
        public static OperationResult<T> Success(T data) => new OperationResult<T>(true, data);
        public static OperationResult<T> Fail(string errorMessage) => new OperationResult<T>(false, default, new List<string> { errorMessage });
        public static OperationResult<T> Fail(IReadOnlyList<string> errors) => new OperationResult<T>(false, default, errors);
    }
}
