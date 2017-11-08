import { Component, Injectable, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

import { GlobalService } from '../../shared/services/global.service';
import { ApiService } from '../../shared/services/api.service';
import { ModalService } from '../../shared/services/modal.service';

import { PasswordValidationDirective } from '../../shared/directives/password-validation.directive';

import { WalletCreation } from '../../shared/classes/wallet-creation';
import { Mnemonic } from '../../shared/classes/mnemonic';

@Component({
  selector: 'create-component',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css'],
})

export class CreateComponent implements OnInit {
  constructor(private globalService: GlobalService, private apiService: ApiService, private genericModalService: ModalService, private router: Router, private fb: FormBuilder) {
    this.buildCreateForm();
  }

  public createWalletForm: FormGroup;
  private newWallet: WalletCreation;
  private mnemonic: string;
  public isCreating: boolean = false;

  ngOnInit() {
    this.getNewMnemonic();
  }

  private buildCreateForm(): void {
    this.createWalletForm = this.fb.group({
      "walletName": ["",
        Validators.compose([
          Validators.required,
          Validators.minLength(1),
          Validators.maxLength(24),
          Validators.pattern(/^[a-zA-Z0-9]*$/)
        ])
      ],
      "walletPassword": ["",
        Validators.required,
        // Validators.compose([
        //   Validators.required,
        //   Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{10,})/)])
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
      'required': 'A wallet name is required.',
      'minlength': 'A wallet name must be at least one character long.',
      'maxlength': 'A wallet name cannot be more than 24 characters long.',
      'pattern': 'Please enter a valid wallet name. [a-Z] and [0-9] are the only characters allowed.'
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

  public onBackClicked() {
    this.router.navigate(["/setup"]);
  }

  public onCreateClicked() {
    this.isCreating = true;
    if (this.mnemonic) {
      this.newWallet = new WalletCreation(
        this.createWalletForm.get("walletName").value,
        this.mnemonic,
        this.createWalletForm.get("walletPassword").value,
      );
      this.createWallets(this.newWallet);
    }
  }

  private getNewMnemonic() {
    this.apiService
      .getNewMnemonic()
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400){
            this.mnemonic = response.json();
          }
        },
        error => {
          console.log(error);
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

  private createWallets(wallet: WalletCreation) {
    this.apiService
      .createBitcoinWallet(wallet)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400){
            // Bitcoin wallet created
          }
        },
        error => {
          console.log(error);
          this.isCreating = false;
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
        () => this.createStratisWallet(wallet)
      )
    ;
  }

  private createStratisWallet(wallet: WalletCreation) {
    this.apiService
      .createStratisWallet(wallet)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400){
            let walletBody = "Your wallet has been created.<br><br>Please write down your 12 word passphrase: <br>" + this.mnemonic + "<br><br>You can recover your wallet on any computer with:<br>- your passphrase<br>- your password<br>- In addition, knowing the approximate date at which you created your wallet will speed up recovery.<br><br>Unlike most other wallets if an attacker acquires your passphrase, he will not be able to hack your wallet without knowing your password. On the contrary, unlike other wallets, you will not be able to recover your wallet only with your passphrase if you lose your password.";
            this.genericModalService.openModal("Wallet Info", walletBody);
            this.router.navigate(['']);
          }
        },
        error => {
          this.isCreating = false;
          console.log(error);
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
