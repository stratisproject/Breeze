import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormGroup, Validators, FormBuilder, AbstractControl } from '@angular/forms';
import { NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import 'rxjs/add/operator/filter';

import './monitor';
import { AdvancedService } from './advanced.service';
import { LoadingState } from './loadingState';
import { SerialDisposable } from '../../../app/shared/classes/serialDisposable';

@Component({
  selector: 'app-advanced',
  templateUrl: './advanced.component.html',
  styleUrls: ['./advanced.component.css']
})
export class AdvancedComponent implements OnInit, OnDestroy {
    private addressCount = '';
    private extPubKeySubs = new SerialDisposable();
    private generateAddressesSubs = new SerialDisposable();
    private resyncSubs = new SerialDisposable();
    private addresses: string[] = [];
    private resyncActioned = false;

    constructor(readonly advancedService: AdvancedService, readonly formBuilder: FormBuilder) { 
        this.setResyncDates();
    }

    icoFormGroup: FormGroup;
    extPubKey = '';
    extPubKeyLoadingState = new LoadingState();  
    generateAddressesLoadingState = new LoadingState();
    resyncLoadingState = new LoadingState();
    resyncDate: NgbDateStruct;
    maxResyncDate: NgbDateStruct;
    minResyncDate: NgbDateStruct;
    get datePickerControl(): AbstractControl { return this.icoFormGroup.get('datePickerControl'); }
    get addressCountControl(): AbstractControl { return this.icoFormGroup.get('addressCountControl'); }
    get showAddressesTick(): boolean { return this.generateAddressesLoadingState.success && 
                                            this.addresses.length && (Number(this.addressCount)===this.addresses.length); }
    get showResyncTick(): boolean { return this.resyncLoadingState.success && this.resyncActioned; }

    ngOnInit() {
        this.registerFormControls();
        this.loadExtPubKey();
    } 

    resync() {
        this.resyncActioned = true;
        const date = new Date(this.resyncDate.year, this.resyncDate.month-1, this.resyncDate.day);
        this.resyncSubs.disposable = this.advancedService.resyncFromDate(date)
                                              .monitor(this.resyncLoadingState)
                                              .subscribe();
    }

    generateAddresses() {
        this.addresses.length = 0;
        this.generateAddressesSubs.disposable = this.advancedService.generateAddresses(Number(this.addressCount))
                                                         .monitor(this.generateAddressesLoadingState)
                                                         .subscribe(x => this.addresses = x);
    }

    private loadExtPubKey() {
        this.extPubKeySubs.disposable = this.advancedService.getExtPubKey()
                                                 .monitor(this.extPubKeyLoadingState)
                                                 .subscribe(x => this.extPubKey = x);
    }

    private registerFormControls() {
        this.icoFormGroup = this.formBuilder.group({
            addressCountControl: ['', [Validators.pattern('^[1-9][0-9]*$')]],
            datePickerControl: [Validators.required] 
        });     

        const control = this.addressCountControl;
        const changes$ = control.valueChanges;
        changes$.filter(_ => control.invalid).subscribe(_ => control.setValue(this.addressCount));
        changes$.filter(_ => !control.invalid).subscribe(_ => this.addressCount = control.value);
    }

    private setResyncDates() {
        const now = new Date();
        this.maxResyncDate = { year: now.getFullYear(), month: now.getMonth()+1, day: now.getDate() }
        this.minResyncDate = { year: now.getFullYear(), month: 1, day: 1 }
    }

    ngOnDestroy() {
        this.extPubKeySubs.dispose();
        this.generateAddressesSubs.dispose();
        this.resyncSubs.dispose();
    }
}
