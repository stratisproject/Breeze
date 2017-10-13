import { NgModule } from '@angular/core';
import { BrowserModule, Title } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpModule } from '@angular/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ClipboardModule } from 'ngx-clipboard';

import { SharedModule } from './shared/shared.module';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { GenericModalComponent } from './shared/components/generic-modal/generic-modal.component';

import { ApiService } from './shared/services/api.service';
import { GlobalService } from './shared/services/global.service';
import { ModalService } from './shared/services/modal.service';

import { SendComponent } from './wallet/send/send.component';
import { SendConfirmationComponent } from './wallet/send/send-confirmation/send-confirmation.component';
import { ReceiveComponent } from './wallet/receive/receive.component';
import { TransactionDetailsComponent } from './wallet/transaction-details/transaction-details.component';
import { LogoutConfirmationComponent } from './wallet/logout-confirmation/logout-confirmation.component';


@NgModule({
  imports: [
    AppRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    ClipboardModule,
    ReactiveFormsModule,
    FormsModule,
    HttpModule,
    NgbModule.forRoot(),
    SharedModule.forRoot()
  ],
  declarations: [
    AppComponent,
    GenericModalComponent,
    LoginComponent,
    LogoutConfirmationComponent,
    SendComponent,
    SendConfirmationComponent,
    ReceiveComponent,
    TransactionDetailsComponent
  ],
  entryComponents: [
    GenericModalComponent,
    SendComponent,
    SendConfirmationComponent,
    ReceiveComponent,
    TransactionDetailsComponent,
    LogoutConfirmationComponent
  ],
  providers: [ ApiService, GlobalService, ModalService, Title ],
  bootstrap: [ AppComponent ]
})

export class AppModule { }
