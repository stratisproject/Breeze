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
		[HttpPost]
        public IActionResult Create([FromBody]WalletCreationModel walletCreation)
        {
            // checks the request is valid
            if (!this.ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
                return this.BadRequest(string.Join(Environment.NewLine, errors));
            }
            
            try
            {
                var mnemonic = this.walletWrapper.Create(walletCreation.Password, walletCreation.FolderPath, walletCreation.Name, walletCreation.Network);
                return this.Json(mnemonic);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine(e);
               
                // indicates that this wallet already exists
                return this.StatusCode((int) HttpStatusCode.Conflict, "This wallet already exists.");
            }            
        }

        public IActionResult Load(WalletLoadModel walletLoad)
        {
            // checks the request is valid
            if (!this.ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
                return this.BadRequest(string.Join(Environment.NewLine, errors));
            }
            
            try
            {
                var wallet = this.walletWrapper.Load(walletLoad.Password, walletLoad.FolderPath, walletLoad.Name);
                return this.Json(wallet);

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
        public IActionResult Recover([FromBody]WalletRecoveryModel walletRecovery)
        {
            // checks the request is valid
            if (!this.ModelState.IsValid)
            {
                var errors = this.ModelState.Values.SelectMany(e => e.Errors.Select(m => m.ErrorMessage));
                return this.BadRequest(string.Join(Environment.NewLine, errors));
            }

            try
            {
                var wallet = this.walletWrapper.Recover(walletRecovery.Password, walletRecovery.FolderPath, walletRecovery.Name, walletRecovery.Network, walletRecovery.Mnemonic);
                return this.Json(wallet);

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
