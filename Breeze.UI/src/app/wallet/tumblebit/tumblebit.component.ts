import { Component, OnInit, OnDestroy } from '@angular/core';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';

import { ApiService } from '../../shared/services/api.service';
import { GlobalService } from '../../shared/services/global.service';
import { WalletInfo } from '../../shared/classes/wallet-info';
import { TumblebitService } from './tumblebit.service';
import { TumblerConnectionRequest } from './classes/tumbler-connection-request';
import { TumbleRequest } from './classes/tumble-request';

import { Observable } from 'rxjs/Rx';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'tumblebit-component',
  providers: [TumblebitService],
  templateUrl: './tumblebit.component.html',
  styleUrls: ['./tumblebit.component.css'],
})

export class TumblebitComponent implements OnInit {
  constructor(private apiService: ApiService, private tumblebitService: TumblebitService, private globalService: GlobalService, private modalService: NgbModal, private fb: FormBuilder) {
    this.buildTumbleForm();
    this.buildConnectForm();
  }

  private confirmedBalance: number;
  private walletBalanceSubscription: Subscription;
  private tumblerParameters: any;
  private tumbleStatus: any;

  private tumbleForm: FormGroup;
  private connectForm: FormGroup;

  private buildTumbleForm(): void {
    this.tumbleForm = this.fb.group({
      'source': ['', Validators.required],
      'destination': ['', Validators.required],
    })

    this.tumbleForm.valueChanges
      .subscribe(data => this.onValueChanged(this.tumbleForm, this.tumbleFormErrors, data));
    
    this.onValueChanged(this.tumbleForm, this.tumbleFormErrors);
  }

  private buildConnectForm(): void {
    this.connectForm = this.fb.group({
      'tumblerAddress': ['', Validators.required],
    })

    this.connectForm.valueChanges
      .subscribe(data => this.onValueChanged(this.connectForm, this.connectFormErrors, data));

    this.onValueChanged(this.connectForm, this.connectFormErrors);
  }


  // TODO: abstract to a shared utility lib
  onValueChanged(originalForm: FormGroup, formErrors: object, data?: any) {
    if (!originalForm) { return; }
    const form = originalForm;
    for (const field in formErrors) {
      formErrors[field] = '';
      const control = form.get(field);
      if (control && control.dirty && !control.valid) {
        const messages = this.validationMessages[field];
        for (const key in control.errors) {
          formErrors[field] += messages[key] + ' ';
        }
      }
    }
  }

  connectFormErrors = {
    'source': '',
    'destination': '',
  };

  tumbleFormErrors = {
    'tumblerAddress': '',
  }

  validationMessages = {
    'source': {
      'required': 'A source address is required',
    },
    'destination': {
      'required': 'A destination address is required.',
    },
    'tumblerAddress': {
      'required': 'A tumbler address is required.',
    }
  }

  private connect() {
    let connection = new TumblerConnectionRequest(
      this.connectForm.get('tumblerAddress').value,
      this.globalService.getNetwork()
    );

    this.tumblebitService
      .connect(connection)
      .subscribe(
        // TODO abstract into shared utility method
        response => {
          if (response.status >= 200 && response.status < 400) {
            this.tumblerParameters = response.json();
          }
        },
        error => {
          console.error(error);
          if (error.status === 0) {
            alert("Something went wrong while connecting to the TumbleBit Client. Please restart the application.");
          } else if (error.status >= 400) {
            if (!error.json().errors[0]) {
              console.error(error);
            }
            else {
              alert(error.json().errors[0].message);
            }
          }
        },
      )
    
    console.log(this.tumblerParameters);
  }

  private tumble() {
    let tumbleRequest = new TumbleRequest(
      this.tumbleForm.get('source').value,
      this.tumbleForm.get('destination').value
    )

    this.tumblebitService
      .tumble(tumbleRequest)
      .subscribe(
        response => {
          if (response.status >= 200 && response.status < 400) {
            this.tumbleStatus = response.json();
          }
        },
        error => {
          console.error(error);
          if (error.status === 0) {
            alert("Something went wrong while connecting to the TumbleBit Client. Please restart the application.");
          } else if (error.status >= 400) {
            if (!error.json().errors[0]) {
              console.error(error);
            }
            else {
              alert(error.json().errors[0].message);
            }
          }
        },
      )
    
      console.log(this.tumbleStatus);
  }

  private stopTumble() {
    console.log("stopping tumble...");
  }

  ngOnInit() {
    this.getWalletBalance();
  };

  ngOnDestroy() {
    this.walletBalanceSubscription.unsubscribe();
  };

  // TODO: move into a shared service
  private getWalletBalance() {
    let walletInfo = new WalletInfo(this.globalService.getWalletName(), this.globalService.getCoinType())
    this.walletBalanceSubscription = this.apiService.getWalletBalance(walletInfo)
      .subscribe(
        response =>  {
          if (response.status >= 200 && response.status < 400) {
              let balanceResponse = response.json();
              this.confirmedBalance = balanceResponse.balances[0].amountConfirmed;
          }
        },
        error => {
          console.log(error);
          if (error.status === 0) {
            alert("Something went wrong while connecting to the API. Make sure your address is correct and try again.");
          } else if (error.status >= 400) {
            if (!error.json().errors[0]) {
              console.log(error);
            }
            else {
              alert(error.json().errors[0].description);
            }
          }
        }
      )
    ;
  };

}
