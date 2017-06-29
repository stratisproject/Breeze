import { Component, OnInit, OnDestroy } from '@angular/core';

import { ApiService } from '../../shared/services/api.service';
import { GlobalService } from '../../shared/services/global.service';

import { WalletInfo } from '../../shared/classes/wallet-info';

import { Observable } from 'rxjs/Rx';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'history-component',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css'],
})

export class HistoryComponent {
  constructor(private apiService: ApiService, private globalService: GlobalService) {}

  private transactions: any;
  private errorMessage: string;
  private walletHistorySubscription: Subscription;

  ngOnInit() {
    this.getHistory();
  }

  ngOnDestroy() {
    this.walletHistorySubscription.unsubscribe();
  }

  private getHistory() {
    let walletInfo = new WalletInfo(this.globalService.getWalletName(), this.globalService.getCoinType())
    this.walletHistorySubscription = this.apiService.getWalletHistory(walletInfo)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            if (response.json().transactionsHistory.length > 0) {
              this.transactions = response.json().transactionsHistory;
            }
          }
        },
        error => {
          console.log(error);
          if (error.status === 0) {
            alert("Something went wrong while connecting to the API. Please restart the application.");
          } else if (error.status >= 400) {
            if (!error.json().errors[0]) {
              console.log(error);
            }
            else {
              alert(error.json().errors[0].description);
            }
          }
        }
      )
    ;
  }
}
