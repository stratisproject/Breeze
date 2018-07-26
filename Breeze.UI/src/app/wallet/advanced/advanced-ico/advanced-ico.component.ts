import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxJs/Subscription';

import { AdvancedService } from './../advanced.service';
import { LoadingState } from './loadingState';

@Component({
  selector: 'app-advanced-ico',
  templateUrl: './advanced-ico.component.html',
  styleUrls: ['./advanced-ico.component.css']
})
export class AdvancedIcoComponent implements OnDestroy {
    private _extPubKey = '';
    private extPubKeySubs: Subscription;
    private _extPubKeyLoadingState: LoadingState = new LoadingState("Failed to get ExtPubKey");

    constructor(private advancedService: AdvancedService) { 
        this.loadExtPubKey();
    }

    public get extPubKey(): string {
        return this._extPubKey;
    }

    public get extPubKeyLoadingState(): LoadingState {
        return this._extPubKeyLoadingState;
    }

    ngOnDestroy() {
        this.extPubKeySubs.unsubscribe();
    }

    private loadExtPubKey() {
        this.extPubKeyLoadingState.loading = true;
        this.extPubKeySubs = this.advancedService.getExtPubKey()
                                                 .subscribe(x => this.onExtPubKey(x), e => this.extPubKeyLoadingState.errored = true);
    }

    private onExtPubKey(key: string) {
        this._extPubKey = key; 
        this.extPubKeyLoadingState.loading = false;
    }
}
