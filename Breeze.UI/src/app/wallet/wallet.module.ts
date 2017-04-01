import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule }   from '@angular/router';

import { WalletComponent } from './wallet.component';
import { MenuComponent } from './menu/menu.component';
import { DashboardComponent } from './menu/dashboard.component';
import { SendComponent } from './send/send.component';
import { ReceiveComponent } from './receive/receive.component';
import { HistoryComponent } from './history/history.component';

import { SharedModule } from '../shared/shared.module';

import { WalletRoutingModule } from './wallet-routing.module';

@NgModule({
  imports: [
    BrowserModule,
    FormsModule,
    WalletRoutingModule,
    SharedModule
  ],
  declarations: [
    WalletComponent,
    MenuComponent,
    DashboardComponent,
    ReceiveComponent,
    SendComponent,
    HistoryComponent,
  ],
  exports: [
    WalletComponent
  ]
})

export class WalletModule { }
