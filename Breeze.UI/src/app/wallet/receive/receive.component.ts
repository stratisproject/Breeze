import { Component, OnInit } from '@angular/core';

import { ApiService } from '../../shared/services/api.service';

import { WalletInfo } from '../../shared/classes/wallet-info';

@Component({
  selector: 'receive-component',
  templateUrl: './receive.component.html',
  styleUrls: ['./receive.component.css'],
})

export class ReceiveComponent {
  constructor(private apiService: ApiService) {}

  private address: any;
  private errorMessage: string;
  private walletInfo: WalletInfo;

  ngOnInit() {
    this.getUnusedReceiveAddresses();
  }

  private getUnusedReceiveAddresses() {
    this.walletInfo = new WalletInfo("Test", 0, "Test")
    this.apiService.getUnusedReceiveAddress(this.walletInfo)
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
