import { Component, OnInit } from '@angular/core';

import { ApiService } from '../../shared/services/api.service';
import { GlobalService } from '../../shared/services/global.service';

import { WalletInfo } from '../../shared/classes/wallet-info';

@Component({
  selector: 'receive-component',
  templateUrl: './receive.component.html',
  styleUrls: ['./receive.component.css'],
})

export class ReceiveComponent {
  constructor(private apiService: ApiService, private globalService: GlobalService) {}

  private address: any;
  private errorMessage: string;

  ngOnInit() {
    this.getUnusedReceiveAddresses();
  }

  private getUnusedReceiveAddresses() {
    let walletInfo = new WalletInfo(this.globalService.getWalletName(), this.globalService.getCoinType())
    this.apiService.getUnusedReceiveAddress(walletInfo)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            this.address = response.json();
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
