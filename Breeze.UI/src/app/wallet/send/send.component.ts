import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';

import { ApiService } from '../../shared/services/api.service';
import { GlobalService } from '../../shared/services/global.service';
import { ModalService } from '../../shared/services/modal.service';

import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

import { TransactionBuilding } from '../../shared/classes/transaction-building';
import { TransactionSending } from '../../shared/classes/transaction-sending';

import { SendConfirmationComponent } from './send-confirmation/send-confirmation.component';

@Component({
  selector: 'send-component',
  templateUrl: './send.component.html',
  styleUrls: ['./send.component.css'],
})

export class SendComponent implements OnInit {
  constructor(private apiService: ApiService, private globalService: GlobalService, private modalService: NgbModal, private genericModalService: ModalService, public activeModal: NgbActiveModal, private fb: FormBuilder) {
    this.buildSendForm();
  }

  public sendForm: FormGroup;
  public coinUnit: string;
  public isSending: boolean = false;
  public estimatedFee: number;
  private transactionHex: string;
  private responseMessage: any;
  private errorMessage: string;
  private transaction: TransactionBuilding;

  ngOnInit() {
    this.coinUnit = this.globalService.getCoinUnit();
  }

  private buildSendForm(): void {
    this.sendForm = this.fb.group({
      "address": ["", Validators.required],
      "amount": ["", Validators.compose([Validators.required, Validators.pattern(/^[0-9]+(\.[0-9]{0,8})?$/)])],
      "fee": ["medium", Validators.required],
      "password": ["", Validators.required]
    });

    this.sendForm.valueChanges
      .subscribe(data => this.onValueChanged(data));

    this.onValueChanged();
  }

  onValueChanged(data?: any) {
    if (!this.sendForm) { return; }
    const form = this.sendForm;
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
    'address': '',
    'amount': '',
    'fee': '',
    'password': ''
  };

  validationMessages = {
    'address': {
      'required': 'An address is required.'
    },
    'amount': {
      'required': 'An amount is required.',
      'pattern': 'Enter a valid amount. Only positive numbers are allowed.'
    },
    'fee': {
      'required': 'A fee is required.'
    },
    'password': {
      'required': 'Your password is required.'
    }
  };

  public getMaxBalance() {
    let data = {
      walletName: this.globalService.getWalletName(),
      feeType: this.sendForm.get("fee").value
    }

    let balanceResponse;

    this.apiService
      .getMaximumBalance(data)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400){
            balanceResponse = response.json();
            console.log(balanceResponse);
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
              this.genericModalService.openModal(null, error.json().errors[0].description);
            }
          }
        },
        () => {
          this.sendForm.patchValue({amount: balanceResponse.maxSpendableAmount});
          this.estimatedFee = balanceResponse.fee;
        }
      )
  };

  public buildTransaction() {
    this.transaction = new TransactionBuilding(
      this.globalService.getWalletName(),
      "account 0",
      this.sendForm.get("password").value,
      this.sendForm.get("address").value,
      this.sendForm.get("amount").value,
      this.sendForm.get("fee").value,
      true
    );

    this.apiService
      .buildTransaction(this.transaction)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400){
            this.responseMessage = response.json();
          }
        },
        error => {
          console.log(error);
          this.isSending = false;
          if (error.status === 0) {
            this.genericModalService.openModal(null, null);
          } else if (error.status >= 400) {
            if (!error.json().errors[0]) {
              console.log(error);
            }
            else {
              this.genericModalService.openModal(null, error.json().errors[0].description);
            }
          }
        },
        () => {
          this.estimatedFee = this.responseMessage.fee;
          this.transactionHex = this.responseMessage.hex;
          if (this.isSending) {
            this.sendTransaction(this.transactionHex);
          }
        }
      )
    ;
  };

  public send() {
    this.isSending = true;
    this.buildTransaction();
  };

  private sendTransaction(hex: string) {
    let transaction = new TransactionSending(hex);
    this.apiService
      .sendTransaction(transaction)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400){
            this.activeModal.close("Close clicked");
          }
        },
        error => {
          console.log(error);
          this.isSending = false;
          if (error.status === 0) {
            this.genericModalService.openModal(null, null);
          } else if (error.status >= 400) {
            if (!error.json().errors[0]) {
              console.log(error);
            }
            else {
              this.genericModalService.openModal(null, error.json().errors[0].description);
            }
          }
        },
        ()=>this.openConfirmationModal()
      )
    ;
  }

  private openConfirmationModal() {
    const modalRef = this.modalService.open(SendConfirmationComponent);
    modalRef.componentInstance.transaction = this.transaction;
  }
}
