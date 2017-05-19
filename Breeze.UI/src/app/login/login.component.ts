import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

import { GlobalService } from '../shared/services/global.service';
import { ApiService } from '../shared/services/api.service';
import { WalletLoad } from '../shared/classes/wallet-load';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  constructor(private globalService: GlobalService, private apiService: ApiService, private router: Router, private fb: FormBuilder) {
    this.buildDecryptForm();
   }

  private openWalletForm: FormGroup;
  private hasWallet: boolean = false;
  private wallets: [string];

  ngOnInit() {
    this.getWalletFiles();
  }

  private buildDecryptForm(): void {
    this.openWalletForm = this.fb.group({
      "selectWallet": ["", Validators.required],
      "password": ["", Validators.required]
    });

    this.openWalletForm.valueChanges
      .subscribe(data => this.onValueChanged(data));

    this.onValueChanged();
  }

  onValueChanged(data?: any) {
    if (!this.openWalletForm) { return; }
    const form = this.openWalletForm;
    for (const field in this.formErrors) {
      this.formErrors[field] = '';
      const control = form.get(field);
      if (control && control.dirty && !control.valid) {
        const messages = this.validationMessages[field];
        for (const key in control.errors) {
          this.formErrors[field] += messages[key] + ' ';
        }
      }
    }
  }

  formErrors = {
    'password': ''
  };

  validationMessages = {
    'password': {
      'required': 'Please enter your password.'
    }
  };

  private updateWalletFileDisplay(walletName: string) {
    this.openWalletForm.patchValue({selectWallet: walletName})
  }

  private onDecryptClicked() {
    this.setGlobalWalletName(this.openWalletForm.get("selectWallet").value);
    let walletLoad = new WalletLoad(
      this.openWalletForm.get("password").value,
      this.globalService.getWalletPath(),
      this.openWalletForm.get("selectWallet").value
      );
    this.loadWallet(walletLoad);
  }

  private onCreateClicked() {
    this.router.navigate(['/setup']);
  }

  private setGlobalWalletName(walletName: string) {
    this.globalService.setWalletName(walletName);
  }

  private getWalletFiles() {
    this.apiService.getWalletFiles()
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            let responseMessage = response.json();
            this.wallets = responseMessage.walletsFiles;
            this.globalService.setWalletPath(responseMessage.walletsPath);
            if (this.wallets.length > 0) {
              this.hasWallet = true;
              for (let wallet in this.wallets) {
                this.wallets[wallet] = this.wallets[wallet].slice(0, -5);
              }
              this.updateWalletFileDisplay(this.wallets[0]);
            } else {
              this.hasWallet = false;
            }
          }
        },
        error => {
          let errorMessage = <any>error;
          if (error.status >= 400) {
            alert(errorMessage);
            console.log(errorMessage);
          }
        }
      );
  }

  private loadWallet(walletLoad: WalletLoad) {
    this.apiService.loadWallet(walletLoad)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            this.globalService.setWalletName(walletLoad.name)
            this.globalService.setCoinType(1);
            this.router.navigate(['/wallet']);
          }
        },
        error => {
          let errorMessage = <any>error;
          if (error.status === 403 && error.json().errors[0].message === "Wrong password, please try again.") {
            alert("Wrong password, try again.");
          } else if (error.status >= 400) {
            alert(errorMessage);
            console.log(errorMessage);
          }
        }
      );
  }
}
