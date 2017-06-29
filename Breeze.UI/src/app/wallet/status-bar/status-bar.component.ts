import { Component, OnInit, OnDestroy } from '@angular/core';

import { ApiService } from '../../shared/services/api.service';
import { GlobalService } from '../../shared/services/global.service';
import { WalletInfo } from '../../shared/classes/wallet-info';

import { Observable } from 'rxjs/Rx';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'status-bar',
  templateUrl: './status-bar.component.html',
  styleUrls: ['./status-bar.component.scss']
})
export class StatusBarComponent implements OnInit {

  private generalWalletInfoSubscription: Subscription;
  private lastBlockSyncedHeight: number;
  private chainTip: number;
  private connectedNodes: number = 0;
  private percentSynced: string;

  constructor(private apiService: ApiService, private globalService: GlobalService) { }

  ngOnInit() {
    this.getGeneralWalletInfo()
  }

  ngOnDestroy() {
    this.generalWalletInfoSubscription.unsubscribe();
  }

  getGeneralWalletInfo() {
    let walletInfo = new WalletInfo(this.globalService.getWalletName(), this.globalService.getCoinType())
    this.generalWalletInfoSubscription = this.apiService.getGeneralInfo(walletInfo)
      .subscribe(
        response =>  {
          if (response.status >= 200 && response.status < 400) {
              let generalWalletInfoResponse = response.json();
              this.lastBlockSyncedHeight = generalWalletInfoResponse.lastBlockSyncedHeight;
              this.chainTip = generalWalletInfoResponse.chainTip;
              this.connectedNodes = generalWalletInfoResponse.connectedNodes;
              this.percentSynced = ((this.lastBlockSyncedHeight / this.chainTip) * 100).toFixed(0);
          }
        },
        error => {
          console.log(error);
          if (error.status === 0) {
            alert("Something went wrong while connecting to the API. Please restart the application.");
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
  }

}
