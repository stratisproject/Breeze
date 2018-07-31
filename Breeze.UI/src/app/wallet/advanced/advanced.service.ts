import { Injectable } from '@angular/core';
import { Observable } from 'rxJs/Observable';
import { HttpClient } from '@angular/common/http'; 

import { GlobalService } from '../../shared/services/global.service';

@Injectable()
export class AdvancedService {
    private readonly accountName = 'account 0';
    private readonly walletName;
    private readonly urlPrefix = 'http://localhost:37221/api/Wallet/';

    constructor(private httpClient: HttpClient, private globalService: GlobalService) { 
        this.walletName = this.globalService.getWalletName();
    }

    public getExtPubKey(): Observable<string> {
        const url = `${this.urlPrefix}extpubkey?WalletName=${this.walletName}&AccountName=${this.accountName}`;
        return this.httpClient.get(url).map(x => x.toString());
    }

    public generateAddresses(count: number): Observable<string[]> {
        const url = `${this.urlPrefix}unusedaddresses?WalletName=${this.walletName}&AccountName=${this.accountName}&Count=${count}`;
        return this.httpClient.get(url).map(x => this.processAddresses(x));
    }

    private processAddresses(response: any): string[] {
        let addresses = new Array<string>();
        for (const address of response) {
            addresses.push(address);
        } 
        return addresses;
    }
}
