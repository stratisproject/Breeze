import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxJs/Subscription';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { DatepickerOptions } from 'ng2-datepicker';

import { AdvancedService } from './advanced.service';
import { LoadingState } from './loadingState';

@Component({
  selector: 'app-advanced',
  templateUrl: './advanced.component.html',
  styleUrls: ['./advanced.component.css']
})
export class AdvancedComponent implements OnInit, OnDestroy {
    private addressCount: string;
    private extPubKeySubs: Subscription;
    private generateAddressesSubs: Subscription;
    private resyncSubs: Subscription;
    private addresses = new Array<string>();
    private resyncActioned = false;

    constructor(private advancedService: AdvancedService, private formBuilder: FormBuilder) { }

    public icoFormGroup: FormGroup;
    public extPubKey = "";
    public resyncDate = new Date();
    public extPubKeyLoadingState = new LoadingState();  
    public generateAddressesLoadingState = new LoadingState();
    public resyncDateOptions: DatepickerOptions;
    public resyncLoadingState = new LoadingState();

    public get datePickerControl() { return this.icoFormGroup.get('datePickerControl'); }
    public get addressCountControl() { return this.icoFormGroup.get('addressCountControl'); }
    public get showAddressesTick() {  
        return this.generateAddressesLoadingState.success && this.addresses.length && (Number(this.addressCount)===this.addresses.length); 
    }
    public get showResyncTick(): boolean { return this.resyncLoadingState.success && this.resyncActioned; }

    ngOnInit() {
        this.registerFormControls();
        this.setResyncDateOptions();
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
        this.resyncSubs = this.advancedService.resyncFromDate(this.resyncDate)
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

    private setResyncDateOptions() {
        const today = new Date();
        this.resyncDateOptions = {
            minDate: new Date(today.getFullYear(),0),
            maxDate: today,
            displayFormat: 'MMMM D[,] YYYY'
        };
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

