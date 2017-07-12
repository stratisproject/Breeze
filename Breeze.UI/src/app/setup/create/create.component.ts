import { Component, Injectable } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

import { GlobalService } from '../../shared/services/global.service';
import { ApiService } from '../../shared/services/api.service';

import { PasswordValidationDirective } from '../../shared/directives/password-validation.directive';

import { WalletCreation } from '../../shared/classes/wallet-creation';
import { Mnemonic } from '../../shared/classes/mnemonic';

@Component({
  selector: 'create-component',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css'],
})

export class CreateComponent {
  constructor(private globalService: GlobalService, private apiService: ApiService, private router: Router, private fb: FormBuilder) {
    this.buildCreateForm();
  }

  private createWalletForm: FormGroup;
  private newWallet: WalletCreation;
  private mnemonic: string;

  private buildCreateForm(): void {
    this.createWalletForm = this.fb.group({
      "walletName": ["",
        Validators.compose([
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(24),
          Validators.pattern(/^[a-zA-Z0-9]*$/)
        ])
      ],
      "walletPassword": ["", 
        Validators.compose([
          Validators.required,
          Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{10,})/)])
        ],
      "walletPasswordConfirmation": ["", Validators.required],
      "selectNetwork": ["test", Validators.required]
    }, {
      validator: PasswordValidationDirective.MatchPassword
    });

    this.createWalletForm.valueChanges
      .subscribe(data => this.onValueChanged(data));

    this.onValueChanged();
  }

  onValueChanged(data?: any) {
    if (!this.createWalletForm) { return; }
    const form = this.createWalletForm;
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
    'walletName': '',
    'walletPassword': '',
    'walletPasswordConfirmation': ''
  };

  validationMessages = {
    'walletName': {
      'required': 'Name is required.',
      'minlength': 'Name must be at least 3 characters long.',
      'maxlength': 'Name cannot be more than 24 characters long.',
      'pattern': 'Enter a valid wallet name. [a-Z] and [0-9] are the only characters allowed.'
    },
    'walletPassword': {
      'required': 'A password is required.',
      'pattern': 'A password must be at least 10 characters long and contain one lowercase and uppercase alphabetical character and a number.'
    },
    'walletPasswordConfirmation': {
      'required': 'Confirm your password.',
      'walletPasswordConfirmation': 'Passwords do not match.'
    }
  };

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
            this.mnemonic = response.json();
            alert("Your wallet has been created.\n\nPlease write down your 12 word passphrase: \n" + this.mnemonic + "\n\nYou will be redirected to the decryption page.");
            this.router.navigate(['']);
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
              alert(error.json().errors[0].message);
            }
          }
        }
      )
    ;
  }
}
