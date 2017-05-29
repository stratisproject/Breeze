using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Breeze.Common.JsonErrors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Breeze.Common.JsonErrors
{
    public class ErrorResult : ObjectResult
    {
	    public ErrorResult(int statusCode, ErrorResponse value) : base(value)
	    {
			StatusCode = statusCode;
		}
	}
}
