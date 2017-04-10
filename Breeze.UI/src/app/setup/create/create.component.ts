import { Component, Injectable } from '@angular/core';

import { ApiService } from '../../shared/api/api.service';

import { WalletCreation } from '../../shared/wallet-creation';
import { Mnemonic } from '../../shared/mnemonic';

@Component({
  selector: 'create-component',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css'],
})

export class CreateComponent {
  constructor(private apiService: ApiService) {}

  private newWallet: WalletCreation;
  private responseMessage: string;

  private createWallet(password: string, network: string, folderPath: string, name: string, ) {
    this.newWallet = new WalletCreation();
    this.newWallet.password = password;
    this.newWallet.network = network;
    this.newWallet.folderPath = folderPath;
    this.newWallet.name = name;

    this.apiService
      .createWallet(this.newWallet)
      .subscribe((response: string) => this.responseMessage = response);
  }
}
