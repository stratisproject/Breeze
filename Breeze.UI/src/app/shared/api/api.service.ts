import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

import { SafeCreation } from '../safe-creation';
import { Mnemonic } from '../mnemonic';

@Injectable()
export class ApiService {
    constructor(private http: Http) {};

    private webApiUrl = 'http://localhost:3000/api/v1';
    private headers = new Headers({'Content-Type': 'application/json'});

    isConnected(): Observable<any> {
      return this.http
        .get(this.webApiUrl + '/safe/connected')
        .map((response:Response) => response.json())
        .catch(this.handleError);
    }

    getWalletStatus(): Observable<any> {
      return this.http
        .get(this.webApiUrl + '/wallet/status')
        .map((response:Response) => response.json())
        .catch(this.handleError);
    }

    getWalletBalance(): Observable<any> {
      return this.http
        .get(this.webApiUrl + '/wallet/balance')
        .map((response:Response) => response.json())
        .catch(this.handleError);
    }

    getWalletHistory(): Observable<any> {
      return this.http
        .get(this.webApiUrl + '/wallet/history')
        .map((response:Response) => response.json())
        .catch(this.handleError);
    }

    createWallet(data: SafeCreation): Observable<any> {
      console.log(JSON.stringify(data));
      return this.http
        .post(this.webApiUrl + 'api/safe', JSON.stringify(data), {headers: this.headers})
        .map(response => response.json());
    }

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
