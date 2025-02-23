namespace DummyProject.Model
{
    public enum ResponseStatus { Success, Failed }

    public class ApiResponse<T>
    {
        public ResponseStatus Status { get; set; }
        public string ResultMessage { get; set; }
        public string ErrorCode { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> Success(T data, string message = "İşlem başarılı") =>
            new() { Status = ResponseStatus.Success, ResultMessage = message, Data = data };

        public static ApiResponse<T> Failed(string message, string errorCode = null) =>
            new() { Status = ResponseStatus.Failed, ResultMessage = message, ErrorCode = errorCode };
    }
}
