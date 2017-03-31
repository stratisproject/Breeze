import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';

import { SetupModule } from './setup/setup.module';
import { WalletModule } from './wallet/wallet.module';
import { SharedModule } from './shared/shared.module';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';

import { ApiService } from './shared/api/api.service';


@NgModule({
  imports: [
    AppRoutingModule,
    BrowserModule,
    HttpModule,
    SetupModule,
    WalletModule,
    SharedModule.forRoot()
  ],
  declarations: [
    AppComponent
  ],
  providers: [ ApiService ],
  bootstrap: [ AppComponent ]
})

export class AppModule { }