import { Injectable } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';

import { SafeCreation } from '../safe-creation';
import { Mnemonic } from '../mnemonic';

@Injectable()
export class ApiService {
    constructor(private http: Http) {};

    private webApiUrl = 'http://localhost:5000/api/v1';
    private headers = new Headers({'Content-Type': 'application/json'});

    isConnected(): Observable<string> {
      return this.http
        .get(this.webApiUrl + '/safe/connected')
        .map(data => data.json())
    }

    createWallet(data: SafeCreation): Observable<any> {
      console.log(JSON.stringify(data));
      return this.http
        .post(this.webApiUrl + 'api/safe', JSON.stringify(data), {headers: this.headers})
        .map(response => response.json());
    }

    private handleError(error: any): Promise<any> {
      console.error('An error occurred', error);
      return Promise.reject(error.message || error);
    }
}
