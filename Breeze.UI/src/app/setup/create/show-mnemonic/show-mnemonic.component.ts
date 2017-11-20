import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { WalletCreation } from '../../../shared/classes/wallet-creation';

import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'app-show-mnemonic',
  templateUrl: './show-mnemonic.component.html',
  styleUrls: ['./show-mnemonic.component.css']
})
export class ShowMnemonicComponent implements OnInit, OnDestroy {

  constructor(private route: ActivatedRoute, private router: Router) { }
  private parameters: any;
  private mnemonic: string;
  private subscription: Subscription;
  private newWallet: WalletCreation;
  public mnemonicArray: string[];

  ngOnInit() {
    this.subscription = this.route.queryParams.subscribe(params => {
      this.newWallet = new WalletCreation(
        params["name"],
        params["mnemonic"],
        params["password"]
      )
    });

    this.showMnemonic();
  }

  private showMnemonic() {
    this.mnemonic = this.newWallet.mnemonic;
    this.mnemonicArray = this.mnemonic.split(" ");
  }

  public onContinueClicked() {
    this.router.navigate(['/setup/create/confirm-mnemonic'], { queryParams : { name: this.newWallet.name, mnemonic: this.newWallet.mnemonic, password: this.newWallet.password }});
  }

  public onCancelClicked() {
    this.router.navigate(['']);
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
