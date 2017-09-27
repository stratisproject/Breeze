import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

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

  public recoverWalletForm: FormGroup;
  public creationDate: Date;
  public isRecovering: boolean = false;
  public maxDate = new Date();
  private walletRecovery: WalletRecovery;
  private bsConfig: Partial<BsDatepickerConfig>;

  ngOnInit() {
    this.bsConfig = Object.assign({}, {showWeekNumbers: false, containerClass: 'theme-blue'});
    console.log(new Date());
  }

  private buildRecoverForm(): void {
    this.recoverWalletForm = this.fb.group({
      "walletName": ["", [
          Validators.required,
          Validators.minLength(1),
          Validators.maxLength(24),
          Validators.pattern(/^[a-zA-Z0-9]*$/)
        ]
      ],
      "walletMnemonic": ["", Validators.required],
      "walletDate": ["", Validators.required],
      "walletPassword": ["", Validators.required],
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
    'walletName': '',
    'walletMnemonic': '',
    'walletDate': '',
    'walletPassword': '',

  };

  validationMessages = {
    'walletName': {
      'required': 'A wallet name is required.',
      'minlength': 'A wallet name must be at least one character long.',
      'maxlength': 'A wallet name cannot be more than 24 characters long.',
      'pattern': 'Please enter a valid wallet name. [a-Z] and [0-9] are the only characters allowed.'
    },
    'walletMnemonic': {
      'required': 'Please enter your 12 word phrase.'
    },
    'walletDate': {
      'required': 'Please choose the date the wallet should sync from.'
    },
    'walletPassword': {
      'required': 'A password is required.'
    },

  };

  public onBackClicked() {
    this.router.navigate(["/setup"]);
  }

  public onRecoverClicked(){
    this.isRecovering = true;

    let recoveryDate = new Date(this.recoverWalletForm.get("walletDate").value);
    recoveryDate.setDate(recoveryDate.getDate() - 1);

    this.walletRecovery = new WalletRecovery(
      this.recoverWalletForm.get("walletName").value,
      this.recoverWalletForm.get("walletMnemonic").value,
      this.recoverWalletForm.get("walletPassword").value,
      this.recoverWalletForm.get("selectNetwork").value,
      recoveryDate
    );
    this.recoverWallets(this.walletRecovery);
  }

  private recoverWallets(recoverWallet: WalletRecovery) {
    this.apiService
      .recoverBitcoinWallet(recoverWallet)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            //Bitcoin Wallet Recovered
          }
        },
        error => {
          this.isRecovering = false;
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
        },
        () => this.recoverStratisWallet(recoverWallet)
      )
    ;
  }

  private recoverStratisWallet(recoverWallet: WalletRecovery){
    this.apiService
      .recoverStratisWallet(recoverWallet)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            alert("Your wallet has been recovered. \nYou will be redirected to the decryption page.");
            this.router.navigate([''])
          }
        },
        error => {
          this.isRecovering = false;
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
