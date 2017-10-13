import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ClipboardModule } from 'ngx-clipboard';

import { WalletComponent } from './wallet.component';
import { MenuComponent } from './menu/menu.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HistoryComponent } from './history/history.component';

import { SharedModule } from '../shared/shared.module';
import { WalletRoutingModule } from './wallet-routing.module';
import { SidebarComponent } from './sidebar/sidebar.component';
import { StatusBarComponent } from './status-bar/status-bar.component';
import { TransactionDetailsComponent } from './transaction-details/transaction-details.component';

@NgModule({
  imports: [
    CommonModule,
    ClipboardModule,
    FormsModule,
    SharedModule.forRoot(),
    NgbModule,
    ReactiveFormsModule,
    WalletRoutingModule
  ],
  declarations: [
    WalletComponent,
    MenuComponent,
    DashboardComponent,
    HistoryComponent,
    SidebarComponent,
    StatusBarComponent
  ],
  exports: []
})

export class WalletModule { }
