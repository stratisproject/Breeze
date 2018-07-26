import { Injectable } from '@angular/core';
import { Observable } from 'rxJs/Observable';
import { HttpClient } from '@angular/common/http'; 

import { GlobalService } from '../../shared/services/global.service';

@Injectable()
export class AdvancedService {
    private readonly accountName = 'account 0';
    private readonly urlPrefix = 'http://localhost:37221/api/Wallet/';

    constructor(private httpClient: HttpClient, private globalService: GlobalService) { }

    public getExtPubKey(): Observable<string> {
        const walletName = this.globalService.getWalletName();
        const url = `${this.urlPrefix}extpubkey?WalletName=${walletName}&AccountName=${this.accountName}`;
        return this.httpClient.get(url).map(x => x.toString());
    }
}
