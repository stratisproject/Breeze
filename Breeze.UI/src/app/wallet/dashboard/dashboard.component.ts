import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { ApiService } from '../../shared/services/api.service';
import { GlobalService } from '../../shared/services/global.service';
import { WalletInfo } from '../../shared/classes/wallet-info';
import { TransactionInfo } from '../../shared/classes/transaction-info';

import { SendComponent } from '../send/send.component';
import { ReceiveComponent } from '../receive/receive.component';
import { TransactionDetailsComponent } from '../transaction-details/transaction-details.component';

import { Observable } from 'rxjs/Rx';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'dashboard-component',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})

export class DashboardComponent implements OnInit {
  constructor(private apiService: ApiService, private globalService: GlobalService, private modalService: NgbModal) {}

  public confirmedBalance: number;
  public unconfirmedBalance: number;
  public transactionArray: TransactionInfo[];
  public coinUnit: string;
  private walletBalanceSubscription: Subscription;
  private walletHistorySubscription: Subscription;

  ngOnInit() {
    this.startSubscriptions();
    this.coinUnit = this.globalService.getCoinUnit();
  };

  ngOnDestroy() {
    this.cancelSubscriptions();
  };

  public openSendDialog() {
    const modalRef = this.modalService.open(SendComponent);
  };

  public openReceiveDialog() {
    const modalRef = this.modalService.open(ReceiveComponent);
  };

  public openTransactionDetailDialog(transaction: TransactionInfo) {
    const modalRef = this.modalService.open(TransactionDetailsComponent);
    modalRef.componentInstance.transaction = transaction;
  }

  private getWalletBalance() {
    let walletInfo = new WalletInfo(this.globalService.getWalletName());
    this.walletBalanceSubscription = this.apiService.getWalletBalance(walletInfo)
      .subscribe(
        response =>  {
          if (response.status >= 200 && response.status < 400) {
              let balanceResponse = response.json();
              this.confirmedBalance = balanceResponse.balances[0].amountConfirmed;
              this.unconfirmedBalance = balanceResponse.balances[0].amountUnconfirmed;
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
              if (error.json().errors[0].description) {
                alert(error.json().errors[0].description);
              } else {
                this.cancelSubscriptions();
                this.startSubscriptions();
              }
            }
          }
        }
      )
    ;
  };

  // todo: add history in seperate service to make it reusable
  private getHistory() {
    let walletInfo = new WalletInfo(this.globalService.getWalletName());
    let historyResponse;
    this.walletHistorySubscription = this.apiService.getWalletHistory(walletInfo)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            if (response.json().transactionsHistory.length > 0) {
              historyResponse = response.json().transactionsHistory;
              this.getTransactionInfo(historyResponse);
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
              if (error.json().errors[0].description) {
                alert(error.json().errors[0].description);
              } else {
                this.cancelSubscriptions();
                this.startSubscriptions();
              }
            }
          }
        }
      )
    ;
  };

  private getTransactionInfo(transactions: any) {
    this.transactionArray = [];

    for (let transaction of transactions) {
      let transactionType;
      if (transaction.type === "send") {
        transactionType = "sent";
      } else if (transaction.type === "received") {
        transactionType = "received";
      }
      let transactionId = transaction.id;
      let transactionAmount = transaction.amount;
      let transactionAddress;
      if (transaction.payments[0]) {
        transactionAddress = transaction.payments[0].destinationAddress;
      } else if (transaction.toAddress) {
        transactionAddress = transaction.toAddress;
      }
      let transactionFee = transaction.fee;
      let transactionConfirmedInBlock = transaction.confirmedInBlock;
      let transactionTimestamp = transaction.timestamp;
      let transactionConfirmed;

      this.transactionArray.push(new TransactionInfo(transactionType, transactionId, transactionAmount, transactionAddress, transactionFee, transactionConfirmedInBlock, transactionTimestamp));
    }
  }

  private cancelSubscriptions() {
    if (this.walletBalanceSubscription) {
      this.walletBalanceSubscription.unsubscribe();
    }

    if(this.walletHistorySubscription) {
      this.walletHistorySubscription.unsubscribe();
    }
  };

  private startSubscriptions() {
    this.getWalletBalance();
    this.getHistory();
  }
}
