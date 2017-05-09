import { Component, OnInit } from '@angular/core';

import { ApiService } from '../../shared/services/api.service';
import { GlobalService } from '../../shared/services/global.service';

import { WalletInfo } from '../../shared/classes/wallet-info';

@Component({
  selector: 'history-component',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css'],
})

export class HistoryComponent {
  constructor(private apiService: ApiService, private globalService: GlobalService) {}

  private transactions: any;
  private errorMessage: string;

  ngOnInit() {
    this.getWalletHistory();
  }

  private getWalletHistory() {
    let walletInfo = new WalletInfo(this.globalService.getWalletName(), this.globalService.getCoinType())
    this.apiService.getWalletHistory(walletInfo)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            this.transactions = response.json().transactions;
          }
        },
        error => {
          if (error.status >= 400) {
            this.errorMessage = <any>error;
            console.log(this.errorMessage);
          }
        }
    );
  }
}
