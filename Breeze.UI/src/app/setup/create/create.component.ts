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
    this.buildCreateForm();
  }

  private createWalletForm: FormGroup;
  private newWallet: WalletCreation;

  private responseMessage: string;
  private errorMessage: string;

  private buildCreateForm(): void {
    this.createWalletForm = this.fb.group({
      "walletName": ["", [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(24)
        ]
      ],
      "walletPassword": ["", Validators.required],
      "selectNetwork": ["main", Validators.required]
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
    'walletPassword': ''
  };

  validationMessages = {
    'walletName': {
      'required':      'Name is required.',
      'minlength':     'Name must be at least 3 characters long.',
      'maxlength':     'Name cannot be more than 24 characters long.'
    },
    'walletPassword': {
      'required': 'A password is required.'
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
