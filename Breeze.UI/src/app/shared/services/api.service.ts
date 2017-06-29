import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions, URLSearchParams} from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/catch';
import "rxjs/add/observable/interval";
import 'rxjs/add/operator/startWith';

import { WalletCreation } from '../classes/wallet-creation';
import { WalletRecovery } from '../classes/wallet-recovery';
import { WalletLoad } from '../classes/wallet-load';
import { WalletInfo } from '../classes/wallet-info';
import { Mnemonic } from '../classes/mnemonic';
import { TransactionBuilding } from '../classes/transaction-building';
import { TransactionSending } from '../classes/transaction-sending';

/**
 * For information on the API specification have a look at our swagger files located at http://localhost:5000/swagger/ when running the daemon
 */
@Injectable()
export class ApiService {
    constructor(private http: Http) {};

    private mockApiUrl = 'http://localhost:3000/api';
    private webApiUrl = 'http://localhost:5000/api';
    private headers = new Headers({'Content-Type': 'application/json'});
    private pollingInterval = 3000;

    /**
     * Gets available wallets at the default path
     */
     getWalletFiles(): Observable<any> {
        return this.http
          .get(this.webApiUrl + '/wallet/files')
          .map((response: Response) => response);
     }

    /**
     * Create a new wallet.
     */
    createWallet(data: WalletCreation): Observable<any> {
      return this.http
        .post(this.webApiUrl + '/wallet/create/', JSON.stringify(data), {headers: this.headers})
        .map((response: Response) => response);
    }

    /**
     * Recover a wallet.
     */
    recoverWallet(data: WalletRecovery): Observable<any> {
      return this.http
        .post(this.webApiUrl + '/wallet/recover/', JSON.stringify(data), {headers: this.headers})
        .map((response: Response) => response);
    }

    /**
     * Load a wallet
     */
    loadWallet(data: WalletLoad): Observable<any> {
      return this.http
        .post(this.webApiUrl + '/wallet/load/', JSON.stringify(data), {headers: this.headers})
        .map((response: Response) => response);
    }

    /**
     * Get wallet status info from the API.
     */
    getWalletStatus(): Observable<any> {
      return this.http
        .get(this.webApiUrl + '/wallet/status')
        .map((response: Response) => response);
    }

    /**
     * Get general wallet info from the API.
     */
    getGeneralInfo(data: WalletInfo): Observable<any> {
      let params: URLSearchParams = new URLSearchParams();
      params.set('Name', data.walletName);

      return Observable
        .interval(this.pollingInterval)
        .startWith(0)
        .switchMap(() => this.http.get(this.webApiUrl + '/wallet/general-info', new RequestOptions({headers: this.headers, search: params})))
        .map((response: Response) => response);
    }

    /**
     * Get wallet balance info from the API.
     */
    getWalletBalance(data: WalletInfo): Observable<any> {
      let params: URLSearchParams = new URLSearchParams();
      params.set('walletName', data.walletName);

      return Observable
        .interval(this.pollingInterval)
        .startWith(0)
        .switchMap(() => this.http.get(this.webApiUrl + '/wallet/balance', new RequestOptions({headers: this.headers, search: params})))
        .map((response: Response) => response);

      // return this.http
      //   .get(this.webApiUrl + '/wallet/balance', new RequestOptions({headers: this.headers, search: params}))
      //   .map((response: Response) => response);
    }

    /**
     * Get a wallets transaction history info from the API.
     */
    getWalletHistory(data: WalletInfo): Observable<any> {
      let params: URLSearchParams = new URLSearchParams();
      params.set('walletName', data.walletName);

      return Observable
        .interval(this.pollingInterval)
        .startWith(0)
        .switchMap(() => this.http.get(this.webApiUrl + '/wallet/history', new RequestOptions({headers: this.headers, search: params})))
        .map((response: Response) => response);

      // return this.http
      //   .get(this.webApiUrl + '/wallet/history', new RequestOptions({headers: this.headers, search: params}))
      //   .map((response: Response) => response);
    }

    /**
     * Get unused receive addresses for a certain wallet from the API.
     */
    getUnusedReceiveAddress(data: WalletInfo): Observable<any> {
      let params: URLSearchParams = new URLSearchParams();
      params.set('walletName', data.walletName);
      params.set('accountName', "account 0"); //temporary

      return this.http
        .get(this.webApiUrl + '/wallet/address', new RequestOptions({headers: this.headers, search: params}))
        .map((response: Response) => response);
    }

    buildTransaction(data: TransactionBuilding): Observable<any> {
      return this.http
        .post(this.webApiUrl + '/wallet/build-transaction/', JSON.stringify(data), {headers: this.headers})
        .map((response: Response) => response);
    }

    sendTransaction(data: TransactionSending): Observable<any> {
      return this.http
        .post(this.webApiUrl + '/wallet/send-transaction/', JSON.stringify(data), {headers: this.headers})
        .map((response: Response) => response);
    }
}
