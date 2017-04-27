import { Component, Injectable } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

import { GlobalService } from '../../shared/services/global.service';
import { ApiService } from '../../shared/services/api.service';

import { WalletCreation } from '../../shared/classes/wallet-creation';
import { Mnemonic } from '../../shared/classes/mnemonic';

@Component({
  selector: 'create-component',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css'],
})

export class CreateComponent {
  constructor(private globalService: GlobalService, private apiService: ApiService, private router: Router, private fb: FormBuilder) {
    this.createWalletForm = fb.group({
      "walletName": ["", Validators.required],
      "walletPassword": ["", Validators.required],
      "selectNetwork": ["main", Validators.required]
    });
  }

  private createWalletForm: FormGroup;
  private newWallet: WalletCreation;

  private responseMessage: string;
  private errorMessage: string;

  private onBackClicked() {
    this.router.navigate(["/setup"]);
  }

  private onCreateClicked() {
    this.newWallet = new WalletCreation(
      this.createWalletForm.get("walletPassword").value,
      this.createWalletForm.get("selectNetwork").value,
      this.globalService.getWalletPath(),
      this.createWalletForm.get("walletName").value
      );
    this.createWallet(this.newWallet);
  }

  private createWallet(wallet: WalletCreation) {
    this.apiService
      .createWallet(wallet)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400){
            this.responseMessage = response.json();
          }
        },
        error => {
          if (error.status >= 400) {
            this.errorMessage = error;
            console.log(this.errorMessage);
          }
        }
      );
  }
}
