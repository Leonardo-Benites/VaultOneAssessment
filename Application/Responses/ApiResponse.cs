using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Responses
{
    public class ApiResponse<T>
    {
        public IEnumerable<T>? DataList { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public int Code { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }

        public ApiResponse()
        {
            Success = true;
        }

        public ApiResponse(T data, string message, int code, bool success, string token = "")
        {
            Data = data;
            Message = message;
            Code = code;
            Success = success;
            Token = token;
        }

        public static ApiResponse<T> SuccessResponse(T? data, string message = "Operation successful", int code = 200, string token = "")
        {
            return new ApiResponse<T>(data, message, code, true, token)
            {
                Data = data,
                Message = message,
                Code = code,
                Success = true,
                Token = token
            };
        }

        public static ApiResponse<IEnumerable<T>> SuccessResponseCollection(IEnumerable<T> data, string message = "Operation successful", int code = 200)
        {
            return new ApiResponse<IEnumerable<T>>(data, message, code, true);
        }

        public static ApiResponse<T> ErrorResponse(string message = "An unexpected error occurred. Please try again later", int code = 500)
        {
            return new ApiResponse<T>(default, message, code, false);
        }
    }
}
