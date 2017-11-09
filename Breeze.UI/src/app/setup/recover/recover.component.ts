import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

import { GlobalService } from '../../shared/services/global.service';
import { ApiService } from '../../shared/services/api.service';
import { ModalService } from '../../shared/services/modal.service';

import { WalletRecovery } from '../../shared/classes/wallet-recovery';

@Component({
  selector: 'app-recover',
  templateUrl: './recover.component.html',
  styleUrls: ['./recover.component.css']
})
export class RecoverComponent implements OnInit {

  constructor(private globalService: GlobalService, private apiService: ApiService, private genericModalService: ModalService, private router: Router, private fb: FormBuilder) {
    this.buildRecoverForm();
  }

  public recoverWalletForm: FormGroup;
  public creationDate: Date;
  public isRecovering: boolean = false;
  public maxDate = new Date();
  public bsConfig: Partial<BsDatepickerConfig>;
  private walletRecovery: WalletRecovery;

  ngOnInit() {
    this.bsConfig = Object.assign({}, {showWeekNumbers: false, containerClass: 'theme-blue'});
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
      recoveryDate
    );
    this.recoverWallets(this.walletRecovery);
  }

  private recoverWallets(recoverWallet: WalletRecovery) {
    let bitcoinErrorMessage = "";
    this.apiService
      .recoverBitcoinWallet(recoverWallet)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            //Bitcoin Wallet Recovered
          }
          this.recoverStratisWallet(recoverWallet, bitcoinErrorMessage);
        },
        error => {
          this.isRecovering = false;
          console.log(error);
          if (error.status === 0) {
            this.genericModalService.openModal(null, null);
          } else if (error.status >= 400) {
            if (!error.json().errors[0]) {
              console.log(error);
            }
            else {
              bitcoinErrorMessage = error.json().errors[0].message;
            }
          }
          this.recoverStratisWallet(recoverWallet, bitcoinErrorMessage);
        }
      )
    ;
  }

  private recoverStratisWallet(recoverWallet: WalletRecovery, bitcoinErrorMessage: string){
    let stratisErrorMessage = "";
    this.apiService
      .recoverStratisWallet(recoverWallet)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            let body = "Your wallet has been recovered. \nYou will be redirected to the decryption page.";
            this.genericModalService.openModal("Wallet Recovered", body);
            this.router.navigate([''])
          }
            this.AlertIfNeeded(bitcoinErrorMessage, stratisErrorMessage);
        },
        error => {
          this.isRecovering = false;
          console.log(error);
          if (error.status === 0) {
            this.genericModalService.openModal(null, null);
          } else if (error.status >= 400) {
            if (!error.json().errors[0]) {
              console.log(error);
            }
            else {
              stratisErrorMessage = error.json().errors[0].message;
            }
          }
          this.AlertIfNeeded(bitcoinErrorMessage, stratisErrorMessage);
        }
      )
    ;
  }

  private AlertIfNeeded(bitcoinErrorMessage: string, stratisErrorMessage: string) {
        if(bitcoinErrorMessage !== "" || stratisErrorMessage !== "") {
          let errorMessage = "<strong>Bitcoin wallet recovery:</strong><br>" + bitcoinErrorMessage + "<br><br><strong>Stratis wallet recovery:</strong><br>" + stratisErrorMessage;
          this.genericModalService.openModal(null, errorMessage);
    }
  }
}
