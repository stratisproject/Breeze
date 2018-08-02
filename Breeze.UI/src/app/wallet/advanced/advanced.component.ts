import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxJs/Subscription';
import { FormGroup, Validators, FormBuilder, AbstractControl } from '@angular/forms';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';

import { AdvancedService } from './advanced.service';
import { LoadingState } from './loadingState';

@Component({
  selector: 'app-advanced',
  templateUrl: './advanced.component.html',
  styleUrls: ['./advanced.component.css']
})
export class AdvancedComponent implements OnInit, OnDestroy {
    private addressCount = "";
    private extPubKeySubs: Subscription;
    private generateAddressesSubs: Subscription;
    private resyncSubs: Subscription;
    private addresses = new Array<string>();
    private resyncActioned = false;

    constructor(private advancedService: AdvancedService, private formBuilder: FormBuilder) { 
        this.setResyncDates();
    }

    public icoFormGroup: FormGroup;
    public extPubKey = "";
    public resyncDate: NgbDateStruct;
    public extPubKeyLoadingState = new LoadingState();  
    public generateAddressesLoadingState = new LoadingState();
    public resyncLoadingState = new LoadingState();
    public maxResyncDate: NgbDateStruct;
    public minResyncDate: NgbDateStruct;
    public get datePickerControl(): AbstractControl { return this.icoFormGroup.get('datePickerControl'); }
    public get addressCountControl(): AbstractControl { return this.icoFormGroup.get('addressCountControl'); }
    public get showAddressesTick(): boolean { return this.generateAddressesLoadingState.success && 
                                            this.addresses.length && (Number(this.addressCount)===this.addresses.length); }
    public get showResyncTick(): boolean { return this.resyncLoadingState.success && this.resyncActioned; }

    ngOnInit() {
        this.registerFormControls();
        this.loadExtPubKey();
    } 

    public generateAddresses() {
        this.addresses = new Array<string>();
        this.generateAddressesLoadingState.loading = true;
        if (this.generateAddressesSubs) { 
            this.generateAddressesSubs.unsubscribe(); 
        }
        this.generateAddressesSubs = this.advancedService.generateAddresses(Number(this.addressCount))
                                                         .subscribe(x => this.onGenerateAddresses(x), 
                                                                    _ => this.generateAddressesLoadingState.errored = true);
    }

    public resync() {
        if (this.resyncSubs) {
            this.resyncSubs.unsubscribe();
        }
        this.resyncLoadingState.loading = this.resyncActioned = true;
        const date = new Date(this.resyncDate.year, this.resyncDate.month-1, this.resyncDate.day);
        this.resyncSubs = this.advancedService.resyncFromDate(date)
                                              .subscribe(_ => this.onResync(),
                                                         _ => this.resyncLoadingState.errored = true);
    }

    private loadExtPubKey() {
        this.extPubKeyLoadingState.loading = true;
        this.extPubKeySubs = this.advancedService.getExtPubKey()
                                                 .subscribe(x => this.onExtPubKey(x), 
                                                            _ => this.extPubKeyLoadingState.errored = true);
    }

    private onExtPubKey(key: string) {
        this.extPubKey = key; 
        this.extPubKeyLoadingState.loading = false;
    }

    private onGenerateAddresses(addresses: string[]) {
        this.generateAddressesLoadingState.loading = false;
        this.addresses = addresses;
    }

    private onResync() {
        this.resyncLoadingState.loading = false;
    }

    private registerFormControls() {
        this.icoFormGroup = this.formBuilder.group({
            addressCountControl: ['', [Validators.pattern('^[1-9][0-9]*$')]],
            datePickerControl: [Validators.required] 
        });     

        this.addressCountControl.valueChanges.subscribe(_ => {
            if (this.addressCountControl.invalid) {
                this.addressCountControl.setValue(this.addressCount); 
            } else {
                this.addressCount = this.addressCountControl.value;
            }
        });
    }

    private setResyncDates() {
        const now = new Date();
        this.maxResyncDate = {year: now.getFullYear(), month: now.getMonth()+1, day: now.getDate()}
        this.minResyncDate = {year: now.getFullYear(), month: 1, day: 1}
    }

    ngOnDestroy() {
        if (this.extPubKeySubs) { 
            this.extPubKeySubs.unsubscribe(); 
        }
        if (this.generateAddressesSubs) { 
            this.generateAddressesSubs.unsubscribe(); 
        }
        if (this.resyncSubs) { 
            this.resyncSubs.unsubscribe(); 
        }
    }
}

