import { Component, OnInit } from '@angular/core';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { LogoutConfirmationComponent } from '../logout-confirmation/logout-confirmation.component';
import { Router } from '@angular/router';

import { GlobalService } from '../../shared/services/global.service';

@Component({
  selector: 'sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent implements OnInit {

  constructor(private globalService: GlobalService, private router: Router, private modalService: NgbModal) { }
  private bitcoinActive: Boolean;

  ngOnInit() {
    if (this.globalService.getCoinName() === "Bitcoin" || this.globalService.getCoinName() === "TestBitcoin") {
      this.bitcoinActive = true;
    } else if (this.globalService.getCoinName() === "Stratis" || this.globalService.getCoinName() === "TestStratis") {
      this.bitcoinActive = false;
    }
  }

  private loadBitcoinWallet() {
    this.toggleClass();
    this.globalService.setCoinName("TestBitcoin");
    this.globalService.setCoinUnit("TBTC");
    this.router.navigate(['/wallet']);
  }

  private loadStratisWallet() {
    this.toggleClass();
    this.globalService.setCoinName("TestStratis");
    this.globalService.setCoinUnit("TSTRAT");
    this.router.navigate(['/wallet/stratis-wallet']);
  }

  private toggleClass(){
    this.bitcoinActive = !this.bitcoinActive;
  }

  private logOut() {
    const modalRef = this.modalService.open(LogoutConfirmationComponent);
  }
}
