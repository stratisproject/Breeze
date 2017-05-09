import { Injectable } from '@angular/core';
import { Http, Headers, Response, RequestOptions, URLSearchParams} from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import { WalletCreation } from '../classes/wallet-creation';
import { WalletRecovery } from '../classes/wallet-recovery';
import { WalletLoad } from '../classes/wallet-load';
import { WalletInfo } from '../classes/wallet-info';
import { Mnemonic } from '../classes/mnemonic';

/**
 * For information on the API specification have a look at our Github:
 * https://github.com/stratisproject/Breeze/blob/master/Breeze.Documentation/ApiSpecification.md
 */
@Injectable()
export class ApiService {
    constructor(private http: Http) {};

    private mockApiUrl = 'http://localhost:3000/api/v1';
    private webApiUrl = 'http://localhost:5000/api/v1';
    private headers = new Headers({'Content-Type': 'application/json'});

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
        .get(this.mockApiUrl + '/wallet/status')
        .map((response: Response) => response);
    }

    /**
     * Get wallet balance info from the API.
     */
    getWalletBalance(): Observable<any> {
      return this.http
        .get(this.mockApiUrl + '/wallet/balance')
        .map((response: Response) => response);
    }

    /**
     * Get a wallets transaction history info from the API.
     */
    getWalletHistory(): Observable<any> {
      return this.http
        .get(this.mockApiUrl + '/wallet/history')
        .map((response: Response) => response);
    }

    /**
     * Get unused receive addresses for a certain wallet from the API.
     */
    getUnusedReceiveAddress(data: WalletInfo): Observable<any> {
      let params: URLSearchParams = new URLSearchParams();
      params.set('walletName', data.walletName);
      params.set('coinType', data.coinType.toString());
      params.set('accountName', data.accountName);
      return this.http
        .get(this.webApiUrl + '/wallet/address', new RequestOptions({headers: this.headers, search: params}))
        .map((response: Response) => response);
    }
}
