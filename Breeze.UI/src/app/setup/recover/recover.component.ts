import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';

import { GlobalService } from '../../shared/services/global.service';
import { ApiService } from '../../shared/services/api.service';

import { WalletRecovery } from '../../shared/classes/wallet-recovery';

@Component({
  selector: 'app-recover',
  templateUrl: './recover.component.html',
  styleUrls: ['./recover.component.css']
})
export class RecoverComponent implements OnInit {

  constructor(private globalService: GlobalService, private apiService: ApiService, private fb: FormBuilder) {
    this.recoverWalletForm = fb.group({
      "walletMnemonic": ["", Validators.required],
      "walletPassword": ["", Validators.required],
      "walletName": ["", Validators.required],
      "selectNetwork": ["main", Validators.required]
    });
  }

  private recoverWalletForm: FormGroup;
  private walletRecovery: WalletRecovery;

  private responseMessage: string;
  private errorMessage: string;

  ngOnInit() {
  }

  private onRecoverClicked(){
    this.walletRecovery = new WalletRecovery(
      this.recoverWalletForm.get("walletMnemonic").value,
      this.recoverWalletForm.get("walletPassword").value,
      this.recoverWalletForm.get("selectNetwork").value,
      this.globalService.getWalletPath(),
      this.recoverWalletForm.get("walletName").value
      );
    this.recoverWallet(this.walletRecovery);
  }

  private recoverWallet(recoverWallet: WalletRecovery) {

    this.apiService
      .recoverWallet(recoverWallet)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            this.responseMessage = response;
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
