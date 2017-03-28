using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using Microsoft.AspNetCore.Mvc;
using Breeze.Wallet.Models;
using Breeze.Wallet.Wrappers;
using Stratis.Bitcoin;


namespace Breeze.Wallet.Controllers
{
    [Route("api/[controller]")]
    public class SafeController : Controller
    {
        private readonly ISafeWrapper safeWrapper;

        public SafeController(ISafeWrapper safeWrapper)
        {
            this.safeWrapper = safeWrapper;
        }
		
		/// <summary>
		/// Creates a new safe on the local machine.
		/// </summary>
		/// <param name="safeCreation">The object containing the parameters used to create the wallet.</param>
		/// <returns>A JSON object containing the mnemonic created for the new wallet.</returns>
		[HttpPost]
        public IActionResult Create([FromBody]SafeCreationModel safeCreation)
        {
            // checks the request is valid
            if (!this.ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
                return this.BadRequest(string.Join(Environment.NewLine, errors));
            }
            
            try
            {
                var mnemonic = this.safeWrapper.Create(safeCreation.Password, safeCreation.FolderPath, safeCreation.Name, safeCreation.Network);
                return this.Json(mnemonic);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine(e);
               
                // indicates that this wallet already exists
                return this.StatusCode((int) HttpStatusCode.Conflict, "This wallet already exists.");
            }            
        }

        public IActionResult Load(SafeLoadModel safeLoad)
        {
            // checks the request is valid
            if (!this.ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
                return this.BadRequest(string.Join(Environment.NewLine, errors));
            }
            
            try
            {
                var safe = this.safeWrapper.Load(safeLoad.Password, safeLoad.FolderPath, safeLoad.Name);
                return this.Json(safe);

            }            
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);

                // indicates that this wallet does not exist
                return this.StatusCode((int)HttpStatusCode.NotFound, "Wallet not found.");
            }
            catch (SecurityException e)
            {
                Console.WriteLine(e);
              
                // indicates that the password is wrong
                return this.StatusCode((int)HttpStatusCode.Forbidden, "Wrong password, please try again.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return this.StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
        }

        [Route("recover")]
        [HttpPost]
        public IActionResult Recover([FromBody]SafeRecoveryModel safeRecovery)
        {
            // checks the request is valid
            if (!this.ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
                return this.BadRequest(string.Join(Environment.NewLine, errors));
            }

            try
            {
                var safe = this.safeWrapper.Recover(safeRecovery.Password, safeRecovery.FolderPath, safeRecovery.Name, safeRecovery.Network, safeRecovery.Mnemonic);
                return this.Json(safe);

            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);

                // indicates that this wallet does not exist
                return this.StatusCode((int)HttpStatusCode.NotFound, "Wallet not found.");
            }
            catch (SecurityException e)
            {
                Console.WriteLine(e);

                // indicates that the password is wrong
                return this.StatusCode((int)HttpStatusCode.Forbidden, "Wrong password, please try again.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return this.StatusCode((int)HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}
