import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'setup-component',
  templateUrl: './setup.component.html',
  styleUrls: ['./setup.component.css'],
})
export class SetupComponent {
  constructor(private router: Router) {}
  private createWallet() {
    this.router.navigate(['/setup/create'])
  }

  private recoverWallet() {
    this.router.navigate(['/setup/recover'])
  }
}
