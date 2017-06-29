import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

import { GlobalService } from '../../shared/services/global.service';
import { ApiService } from '../../shared/services/api.service';

import { WalletRecovery } from '../../shared/classes/wallet-recovery';

@Component({
  selector: 'app-recover',
  templateUrl: './recover.component.html',
  styleUrls: ['./recover.component.css']
})
export class RecoverComponent implements OnInit {

  constructor(private globalService: GlobalService, private apiService: ApiService, private router: Router, private fb: FormBuilder) {
    this.buildRecoverForm();

  }

  private recoverWalletForm: FormGroup;
  private creationDate: Date;
  private walletRecovery: WalletRecovery;

  private responseMessage: string;
  private errorMessage: string;

  ngOnInit() {
  }

  private buildRecoverForm(): void {
    this.recoverWalletForm = this.fb.group({
      "walletMnemonic": ["", Validators.required],
      "walletPassword": ["", Validators.required],
      "walletName": ["", [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(24)
        ]
      ],
      "selectNetwork": ["test", Validators.required]
    });

    this.recoverWalletForm.valueChanges
      .subscribe(data => this.onValueChanged(data));

    this.onValueChanged();
  }

  onValueChanged(data?: any) {
    if (!this.recoverWalletForm) { return; }
    const form = this.recoverWalletForm;
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
    'walletMnemonic': '',
    'walletPassword': '',
    'walletName': ''
  };

  validationMessages = {
    'walletMnemonic': {
      'required': 'Please enter your 12 word phrase.'
    },
    'walletPassword': {
      'required': 'A password is required.'
    },
    'walletName': {
      'required':      'Name is required.',
      'minlength':     'Name must be at least 3 characters long.',
      'maxlength':     'Name cannot be more than 24 characters long.'
    }
  };

  private onBackClicked() {
    this.router.navigate(["/setup"]);
  }

  private onRecoverClicked(){
    this.walletRecovery = new WalletRecovery(
      this.recoverWalletForm.get("walletMnemonic").value,
      this.recoverWalletForm.get("walletPassword").value,
      this.recoverWalletForm.get("selectNetwork").value,
      this.globalService.getWalletPath(),
      this.recoverWalletForm.get("walletName").value,
      this.creationDate
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
            alert("Your wallet has been recovered. \nYou will be redirected to the decryption page.");
            this.router.navigate([''])
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
              alert(error.json().errors[0].description);
            }
          }
        }
      );
  }
}
