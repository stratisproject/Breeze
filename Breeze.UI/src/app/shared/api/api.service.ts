import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import { WalletCreation } from '../wallet-creation';
import { WalletRecovery } from '../wallet-recovery';
import { Mnemonic } from '../mnemonic';

/**
 * For information on the API specification have a look at our Github:
 * https://github.com/stratisproject/Breeze/blob/master/Breeze.Documentation/ApiSpecification.md
 */
@Injectable()
export class ApiService {
    constructor(private http: Http) {};

    private webApiUrl = 'http://localhost:3000/api/v1';
    private headers = new Headers({'Content-Type': 'application/json'});

    /**
     * Create a new wallet.
     */
    createWallet(data: WalletCreation): Observable<any> {
      return this.http
        .post(this.webApiUrl + '/wallet/create/', JSON.stringify(data), {headers: this.headers})
        .map(response => response.json());
    }

    /**
     * Recover a wallet.
     */
    recoverWallet(data: WalletRecovery): Observable<any> {
      return this.http
        .post(this.webApiUrl + '/wallet/recover/', JSON.stringify(data), {headers: this.headers})
        .map(response => response.json());
    }

    /**
     * Load a wallet
     */
    loadWallet(password: string): Observable<any> {
      return this.http
        .get(this.webApiUrl + '/wallet/load/', {headers: this.headers, body: JSON.stringify(password)})
        .map(response => response.json())
        .catch(this.handleError);
    }

    /**
     * Get wallet status info from the API.
     */
    getWalletStatus(): Observable<any> {
      return this.http
        .get(this.webApiUrl + '/wallet/status')
        .map((response:Response) => response.json())
        .catch(this.handleError);
    }

    /**
     * Get wallet balance info from the API. 
     */
    getWalletBalance(): Observable<any> {
      return this.http
        .get(this.webApiUrl + '/wallet/balance')
        .map((response:Response) => response.json())
        .catch(this.handleError);
    }

    /**
     * Get a wallets transaction history info from the API.
     */
    getWalletHistory(): Observable<any> {
      return this.http
        .get(this.webApiUrl + '/wallet/history')
        .map((response:Response) => response.json())
        .catch(this.handleError);
    }

    /**
     * Get unused receive addresses for a certain wallet from the API.
     */
    getUnusedReceiveAddresses(): Observable<any> {
      return this.http
        .get(this.webApiUrl + '/wallet/receive')
        .map((response:Response) => response.json())
        .catch(this.handleError);
    }
    
    /**
     * Handle errors from the API.
     * @param error 
     */
    private handleError (error: Response | any) {
    let errMsg: string;
    if (error instanceof Response) {
      const body = error.json() || '';
      const err = body.error || JSON.stringify(body);
      errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
    } else {
      errMsg = error.message ? error.message : error.toString();
    }
    console.error(errMsg);
    return Observable.throw(errMsg);
  }
}
