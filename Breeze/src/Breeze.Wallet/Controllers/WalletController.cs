using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using Breeze.Wallet.Errors;
using Microsoft.AspNetCore.Mvc;
using Breeze.Wallet.Models;
using Breeze.Wallet.Wrappers;

namespace Breeze.Wallet.Controllers
{
	[Route("api/v{version:apiVersion}/[controller]")]
    public class WalletController : Controller
    {
        private readonly IWalletWrapper walletWrapper;

        public WalletController(IWalletWrapper walletWrapper)
        {
            this.walletWrapper = walletWrapper;
        }

		/// <summary>
		/// Creates a new wallet on the local machine.
		/// </summary>
		/// <param name="walletCreation">The object containing the parameters used to create the wallet.</param>
		/// <returns>A JSON object containing the mnemonic created for the new wallet.</returns>
		[Route("create")]
		[HttpPost]
        public IActionResult Create([FromBody]WalletCreationRequest walletCreation)
        {
            // checks the request is valid
            if (!this.ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
			}
            
            try
            {
                var mnemonic = this.walletWrapper.Create(walletCreation.Password, walletCreation.FolderPath, walletCreation.Name, walletCreation.Network);
                return this.Json(mnemonic);
            }
            catch (NotSupportedException e)
            {
				// indicates that this wallet already exists
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.Conflict, "This wallet already exists.", e.ToString());
            }            
        }

		[Route("load")]
		[HttpPost]
        public IActionResult Load([FromBody]WalletLoadRequest walletLoad)
        {
            // checks the request is valid
            if (!this.ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
			}
            
            try
            {
                this.walletWrapper.Load(walletLoad.Password, walletLoad.FolderPath, walletLoad.Name);
                return this.Json(new SuccessModel {Success = true});
            }            
            catch (FileNotFoundException e)
            {
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.NotFound, "Wallet not found.", e.ToString());
			}
            catch (SecurityException e)
            {
				// indicates that the password is wrong
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.Forbidden, "Wrong password, please try again.", e.ToString());				
            }
            catch (Exception e)
            {
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
			}
        }

        [Route("recover")]
        [HttpPost]
        public IActionResult Recover([FromBody]WalletRecoveryRequest walletRecovery)
        {
            // checks the request is valid
            if (!this.ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
			}

            try
            {
                this.walletWrapper.Recover(walletRecovery.Password, walletRecovery.FolderPath, walletRecovery.Name, walletRecovery.Network, walletRecovery.Mnemonic);
	            return this.Json(new SuccessModel {Success = true});

            }
            catch (FileNotFoundException e)
            {
				// indicates that this wallet does not exist
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.Conflict, "This wallet already exists.", e.ToString());	
            }
            catch (Exception e)
            {
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
			}
        }

	    [Route("general-info")]
	    [HttpGet]
	    public IActionResult GetGeneralInfo()
	    {
			// checks the request is valid
			if (!this.ModelState.IsValid)
			{
				var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
			}

			try
			{				
				return this.Json(this.walletWrapper.GetGeneralInfo());

			}			
			catch (Exception e)
			{
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
			}
		}

		[Route("sensitive-info")]
		[HttpGet]
		public IActionResult GetSensitiveInfo([FromBody] WalletSensitiveRequest request)
		{
			// checks the request is valid
			if (!this.ModelState.IsValid)
			{
				var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
			}

			try
			{
				return this.Json(this.walletWrapper.GetSensitiveInfo(request.Password));

			}
			catch (Exception e)
			{
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
			}
		}

		[Route("status-info")]
		[HttpGet]
		public IActionResult GetStatusInfo()
		{
			// checks the request is valid
			if (!this.ModelState.IsValid)
			{
				var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
			}

			try
			{
				return this.Json(this.walletWrapper.GetStatusInfo());

			}
			catch (Exception e)
			{
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
			}
		}

		[Route("history")]
		[HttpGet]
		public IActionResult GetHistory([FromQuery] AccountName model)
		{
			// checks the request is valid
			if (!this.ModelState.IsValid)
			{
				var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
			}

			try
			{
				return this.Json(this.walletWrapper.GetHistory(model.Name));

			}
			catch (Exception e)
			{
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
			}
		}

		[Route("balance")]
		[HttpGet]
		public IActionResult GetBalance([FromQuery] AccountName model)
		{
			// checks the request is valid
			if (!this.ModelState.IsValid)
			{
				var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
			}

			try
			{
				return this.Json(this.walletWrapper.GetBalance(model.Name));

			}
			catch (Exception e)
			{
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
			}
		}

		[Route("build-transaction")]
		[HttpPost]
		public IActionResult BuildTransaction([FromQuery] AccountName model, [FromBody] BuildTransactionRequest request)
		{
			// checks the request is valid
			if (!this.ModelState.IsValid)
			{
				var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
			}

			try
			{
				return this.Json(this.walletWrapper.BuildTransaction(model.Name, request.Password, request.Address, request.Amount, request.FeeType, request.AllowUnconfirmed));

			}
			catch (Exception e)
			{
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
			}
		}

		[Route("send-transaction")]
		[HttpPost]
		public IActionResult SendTransaction([FromBody] SendTransactionRequest request)
		{
			// checks the request is valid
			if (!this.ModelState.IsValid)
			{
				var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, "Formatting error", string.Join(Environment.NewLine, errors));
			}

			try
			{
				var result = this.walletWrapper.SendTransaction(request.Hex);
				if (result)
				{
					return this.Ok();
				}

				return this.StatusCode((int)HttpStatusCode.BadRequest);
			}
			catch (Exception e)
			{
				return ErrorHelpers.BuildErrorResponse(HttpStatusCode.BadRequest, e.Message, e.ToString());
			}
		}
	}
}
