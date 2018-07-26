import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ClipboardModule } from 'ngx-clipboard';
import { HttpClientModule } from '@angular/common/http';

import { WalletComponent } from './wallet.component';
import { MenuComponent } from './menu/menu.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { HistoryComponent } from './history/history.component';

import { SharedModule } from '../shared/shared.module';
import { WalletRoutingModule } from './wallet-routing.module';
import { SidebarComponent } from './sidebar/sidebar.component';
import { StatusBarComponent } from './status-bar/status-bar.component';
import { AdvancedComponent } from './advanced/advanced.component';
import { AdvancedIcoComponent } from './advanced/advanced-ico/advanced-ico.component';

import { AdvancedService } from '../wallet/advanced/advanced.service';
import { FeedbackComponent } from './advanced/feedback/feedback.component';

@NgModule({
  imports: [
    HttpClientModule,
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
    StatusBarComponent,
    AdvancedComponent,
    AdvancedIcoComponent,
    FeedbackComponent
  ],
  providers: [
    AdvancedService
  ],
  exports: []
})

export class WalletModule { }
