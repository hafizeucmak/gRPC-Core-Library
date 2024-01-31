﻿using Grpc.Core;

namespace LibraryManagement.Common.Base
{
    public class ErrorResponse : IErrorResponse
    {
        public int Code { get; set; }

        public string? Message { get; set; }

        public string? InnerException { get; set; }

        public string? ExceptionContent { get; set; }

        public StatusCode? StatusCode { get; set; }
    }
}
