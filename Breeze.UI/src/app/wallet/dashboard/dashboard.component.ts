import { Component, OnInit, Input } from '@angular/core';
import {NgbModal, NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

import { ApiService } from '../../shared/services/api.service';
import { GlobalService } from '../../shared/services/global.service';
import { WalletInfo } from '../../shared/classes/wallet-info';

import { SendComponent } from '../send/send.component';
import { ReceiveComponent } from '../receive/receive.component';

@Component({
  selector: 'dashboard-component',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})

export class DashboardComponent {
  constructor(private apiService: ApiService, private globalService: GlobalService, private modalService: NgbModal) {}

  private confirmedBalance: number;
  private unconfirmedBalance: number;
  private transactions: any;

  ngOnInit() {
      this.getWalletBalance();
      this.getHistory();
  };

  private openSendDialog() {
    const modalRef = this.modalService.open(SendComponent);
  };

  private openReceiveDialog() {
    const modalRef = this.modalService.open(ReceiveComponent);
  };

  private getWalletBalance() {
    let walletInfo = new WalletInfo(this.globalService.getWalletName(), this.globalService.getCoinType())
    this.apiService.getWalletBalance(walletInfo)
        .subscribe(
            response =>  {
                if (response.status >= 200 && response.status < 400) {
                    let balanceResponse = response.json();
                    this.confirmedBalance = balanceResponse.balances[0].amountConfirmed;
                    this.unconfirmedBalance = balanceResponse.balances[0].amountUnconfirmed;
                }
            },
            error => {
                if (error.status >= 400) {
                    let errorMessage = <any>error;
                    console.log(errorMessage);
                }
            }
      );
  };

  private getHistory() {
    let walletInfo = new WalletInfo(this.globalService.getWalletName(), this.globalService.getCoinType())
    this.apiService.getWalletHistory(walletInfo)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            if (response.json().transactionsHistory.length > 0) {
              this.transactions = response.json().transactionsHistory;
            }
          }
        },
        error => {
          if (error.status >= 400) {
            let errorMessage = <any>error;
            console.log(errorMessage);
          }
        }
    );
  };
}
