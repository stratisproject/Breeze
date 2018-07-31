import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxJs/Subscription';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import 'rxjs/add/operator/filter';
import { timer } from 'rxjs/observable/timer';

import { AdvancedService } from './../advanced.service';
import { LoadingState } from './loadingState';

@Component({
  selector: 'app-advanced-ico',
  templateUrl: './advanced-ico.component.html',
  styleUrls: ['./advanced-ico.component.css']
})
export class AdvancedIcoComponent implements OnInit, OnDestroy {
    private addressCount: string;
    private extPubKeySubs: Subscription;
    private generateAddressesSubs: Subscription;
    private addresses = new Array<string>();

    constructor(private advancedService: AdvancedService, private formBuilder: FormBuilder) { }

    ngOnInit() {
        this.registerFormControls();
        this.loadExtPubKey();
    }

    public icoFormGroup: FormGroup;
    public extPubKey = "";
    public extPubKeyLoadingState = new LoadingState();  
    public generateAddressesLoadingState = new LoadingState();

    public get addressCountControl() { return this.icoFormGroup.get('addressCountControl'); }
    public get showTick() { 
        return this.generateAddressesLoadingState.success && this.addresses.length && (Number(this.addressCount)===this.addresses.length) 
    }

    public generateAddresses() {
        this.internalGenerateAddresses();
    }

    private loadExtPubKey() {
        this.extPubKeyLoadingState.loading = true;
        this.extPubKeySubs = this.advancedService.getExtPubKey()
                                                 .subscribe(x => this.onExtPubKey(x), 
                                                            _ => this.extPubKeyLoadingState.errored = true);
    }

    private internalGenerateAddresses() {
        this.addresses = new Array<string>();
        this.generateAddressesLoadingState.loading = true;
        if (this.generateAddressesSubs) { 
            this.generateAddressesSubs.unsubscribe(); 
        }
        this.generateAddressesSubs = this.advancedService.generateAddresses(Number(this.addressCount))
                                                         .subscribe(x => this.onGenerateAddresses(x), 
                                                                    _ => this.generateAddressesLoadingState.errored = true);
    }

    private onExtPubKey(key: string) {
        this.extPubKey = key; 
        this.extPubKeyLoadingState.loading = false;
    }

    private onGenerateAddresses(addresses: string[]) {
        this.generateAddressesLoadingState.loading = false;
        this.addresses = addresses;
    }

    private registerFormControls() {
        this.icoFormGroup = this.formBuilder.group({
            addressCountControl: ['', [Validators.pattern('^[1-9][0-9]*$')]]
        });   

        this.addressCountControl.valueChanges.subscribe(_ => {
            if (this.addressCountControl.invalid) {
                this.addressCountControl.setValue(this.addressCount); 
            } else {
                this.addressCount = this.addressCountControl.value;
            }
        });
    }

    ngOnDestroy() {
        if (this.extPubKeySubs) {
            this.extPubKeySubs.unsubscribe();
        }
        if (this.generateAddressesSubs) {
            this.generateAddressesSubs.unsubscribe();
        }
    }
}
