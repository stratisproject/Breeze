using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Breeze.Wallet.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Breeze.Wallet.Errors
{
    public class ErrorResult : ObjectResult
    {
	    public ErrorResult(int statusCode, ErrorResponse value) : base(value)
	    {
			StatusCode = statusCode;
		}
	}
}
