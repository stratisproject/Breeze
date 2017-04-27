import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { HttpModule } from '@angular/http';

import { SetupModule } from './setup/setup.module';
import { WalletModule } from './wallet/wallet.module';
import { SharedModule } from './shared/shared.module';

import { AppRoutingModule } from './app-routing.module';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';

import { ApiService } from './shared/services/api.service';
import { GlobalService } from './shared/services/global.service';


@NgModule({
  imports: [
    AppRoutingModule,
    BrowserModule,
    ReactiveFormsModule,
    FormsModule,
    HttpModule,
    SharedModule.forRoot()
  ],
  declarations: [
    AppComponent,
    LoginComponent
  ],
  providers: [ ApiService, GlobalService ],
  bootstrap: [ AppComponent ]
})

export class AppModule { }
