import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Subscription } from 'rxJs/Subscription';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';

import 'rxjs/add/operator/filter';

import { AdvancedService } from './../advanced.service';
import { LoadingState } from './loadingState';

@Component({
  selector: 'app-advanced-ico',
  templateUrl: './advanced-ico.component.html',
  styleUrls: ['./advanced-ico.component.css']
})
export class AdvancedIcoComponent implements OnInit, OnDestroy {
    public icoFormGroup: FormGroup;
    private _extPubKey = '';
    private addressCount: string;
    private extPubKeySubs: Subscription;
    private _extPubKeyLoadingState: LoadingState = new LoadingState();

    constructor(private advancedService: AdvancedService, private formBuilder: FormBuilder) { 
        this.loadExtPubKey();
    }

    ngOnInit() {
        this.registerFormControls();
    }

    public get extPubKey(): string { return this._extPubKey; }
    public get extPubKeyLoadingState(): LoadingState { return this._extPubKeyLoadingState; }
    public get addressCountControl() { return this.icoFormGroup.get('addressCountControl'); }

    private loadExtPubKey() {
        this.extPubKeyLoadingState.loading = true;
        this.extPubKeySubs = this.advancedService.getExtPubKey()
                                                 .subscribe(x => this.onExtPubKey(x), e => this.extPubKeyLoadingState.errored = true);
    }

    private onExtPubKey(key: string) {
        this._extPubKey = key; 
        this.extPubKeyLoadingState.loading = false;
    }

    private registerFormControls() {
        this.icoFormGroup = this.formBuilder.group({
            addressCountControl: ["", [Validators.required, Validators.pattern('^[1-9][0-9]*$')]]
        });   
        let ignore = false;
        this.addressCountControl.valueChanges.filter(_ => !ignore).subscribe(_ => {
            if (this.addressCountControl.invalid && this.addressCountControl.value) {
                ignore = true;
                this.addressCountControl.setValue(this.addressCount);
                ignore = false;
            } else {
                this.addressCount = this.addressCountControl.value;
            }
        });
    }

    ngOnDestroy() {
        this.extPubKeySubs.unsubscribe();
    }
}
