import { Component, Injectable } from '@angular/core';

import { ApiService } from '../../shared/api/api.service';

import { SafeCreation } from '../../shared/safe-creation';
import { Mnemonic } from '../../shared/mnemonic';

@Component({
  selector: 'create-component',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css'],
})

export class CreateComponent {
  constructor(private apiService: ApiService) {}

  private newWallet: SafeCreation;
  private body: string;

  private createWallet(password: string, network: string, folderPath: string, name: string, ) {
    this.newWallet.password = password;
    this.newWallet.network = network;
    this.newWallet.folderPath = folderPath;
    this.newWallet.name = name;

    this.apiService
      .createWallet(this.newWallet)
      //.map(res => {let body = res.text()})
      .subscribe((response: string) => this.body = response,
      () => console.log("createWallet() complete from init"));
  }
}
