import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

import { GlobalService } from '../shared/services/global.service';
import { ApiService } from '../shared/services/api.service';
import { ModalService } from '../shared/services/modal.service';

import { WalletLoad } from '../shared/classes/wallet-load';
import { WalletInfo } from '../shared/classes/wallet-info';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  constructor(private globalService: GlobalService, private apiService: ApiService, private genericModalService: ModalService, private router: Router, private fb: FormBuilder) {
    this.buildDecryptForm();
  }

  public hasWallet: boolean = false;
  public isDecrypting = false;
  private openWalletForm: FormGroup;
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
                this.wallets[wallet] = this.wallets[wallet].slice(0, -12);
              }
              this.updateWalletFileDisplay(this.wallets[0]);
            } else {
              this.hasWallet = false;
            }
          }
        },
        error => {
          if (error.status === 0) {
            this.genericModalService.openModal(null, null);
          } else if (error.status >= 400) {
            if (!error.json().errors[0]) {
              console.log(error);
            }
            else {
              this.genericModalService.openModal(null, error.json().errors[0].message);
            }
          }
        }
      )
    ;
  }

  private updateWalletFileDisplay(walletName: string) {
    this.openWalletForm.patchValue({selectWallet: walletName})
  }

  private onCreateClicked() {
    this.router.navigate(['/setup']);
  }

  private onEnter() {
    if (this.openWalletForm.valid) {
      this.onDecryptClicked();
    }
  }

  private onDecryptClicked() {
    this.isDecrypting = true;
    this.globalService.setWalletName(this.openWalletForm.get("selectWallet").value);
    this.getCurrentNetwork();
    let walletLoad = new WalletLoad(
      this.openWalletForm.get("selectWallet").value,
      this.openWalletForm.get("password").value
    );
    this.loadWallets(walletLoad);
  }

  private loadWallets(walletLoad: WalletLoad) {
    this.apiService.loadBitcoinWallet(walletLoad)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            this.globalService.setWalletName(walletLoad.name);
          }
        },
        error => {
          this.isDecrypting = false;
          if (error.status === 0) {
            this.genericModalService.openModal(null, null);
          } else if (error.status >= 400) {
            if (!error.json().errors[0]) {
              console.log(error);
            }
            else {
              this.genericModalService.openModal(null, error.json().errors[0].message);
            }
          }
        },
        () => this.loadStratisWallet(walletLoad)
      )
    ;
  }

  private loadStratisWallet(walletLoad: WalletLoad) {
    this.apiService.loadStratisWallet(walletLoad)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            // Navigate to the wallet section
            this.router.navigate(['/wallet']);
          }
        },
        error => {
          this.isDecrypting = false;
          if (error.status === 0) {
            this.genericModalService.openModal(null, null);
          } else if (error.status >= 400) {
            if (!error.json().errors[0]) {
              console.log(error);
            }
            else {
              this.genericModalService.openModal(null, error.json().errors[0].message);
            }
          }
        }
      )
    ;
  }

  private getCurrentNetwork() {
    let walletInfo = new WalletInfo(this.globalService.getWalletName())
    this.apiService.getGeneralInfoOnce(walletInfo)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            let responseMessage = response.json();
            this.globalService.setNetwork(responseMessage.network);
            if (responseMessage.network === "Main") {
              this.globalService.setCoinName("Bitcoin");
              this.globalService.setCoinUnit("BTC");
            } else if (responseMessage.network === "TestNet") {
              this.globalService.setCoinName("TestBitcoin");
              this.globalService.setCoinUnit("TBTC");
            }
          }
        },
        error => {
          if (error.status === 0) {
            this.genericModalService.openModal(null, null);
          } else if (error.status >= 400) {
            if (!error.json().errors[0]) {
              console.log(error);
            }
            else {
              this.genericModalService.openModal(null, error.json().errors[0].message);
            }
          }
        }
      )
    ;
  }
}
