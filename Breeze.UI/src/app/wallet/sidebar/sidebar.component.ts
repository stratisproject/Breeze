import { Component, OnInit } from '@angular/core';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { LogoutConfirmationComponent } from '../logout-confirmation/logout-confirmation.component';
import { Router } from '@angular/router';

import { ApiService } from '../../shared/services/api.service';
import { GlobalService } from '../../shared/services/global.service';
import { ModalService } from '../../shared/services/modal.service';

import { WalletInfo } from '../../shared/classes/wallet-info';

@Component({
  selector: 'sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {

  constructor(private globalService: GlobalService, private apiService: ApiService, private router: Router, private modalService: NgbModal, private genericModalService: ModalService) { }
  public bitcoinActive: boolean;
  public stratisActive: boolean;

  ngOnInit() {
    if (this.globalService.getCoinName() === "Bitcoin" || this.globalService.getCoinName() === "TestBitcoin") {
      this.bitcoinActive = true;
      this.stratisActive = false;
    } else if (this.globalService.getCoinName() === "Stratis" || this.globalService.getCoinName() === "TestStratis") {
      this.bitcoinActive = false;
      this.stratisActive = true;
    }
  }

  public loadBitcoinWallet() {
    let currentNetwork = this.globalService.getNetwork();
    if (currentNetwork === "Main") {
      this.globalService.setCoinName("Bitcoin");
      this.globalService.setCoinUnit("BTC");
    } else if (currentNetwork === "TestNet"){
      this.globalService.setCoinName("TestBitcoin");
      this.globalService.setCoinUnit("TBTC");
    }

    this.bitcoinActive = true;
    this.stratisActive = false;
    this.router.navigate(['/wallet']);
  }

  public loadStratisWallet() {
    let currentNetwork = this.globalService.getNetwork();
    if (currentNetwork === "Main") {
      this.globalService.setCoinName("Stratis");
      this.globalService.setCoinUnit("STRAT");
    } else if (currentNetwork === "TestNet"){
      this.globalService.setCoinName("TestStratis");
      this.globalService.setCoinUnit("TSTRAT");
    }

    this.bitcoinActive = false;
    this.stratisActive = true;
    this.router.navigate(['/wallet/stratis-wallet']);
  }

  public logOut() {
    const modalRef = this.modalService.open(LogoutConfirmationComponent);
  }
}
