import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { WalletComponent }   from './wallet.component';
import { SendComponent } from './send/send.component';
import { ReceiveComponent } from './receive/receive.component';
import { HistoryComponent } from './history/history.component';

const routes: Routes = [
  { path: '', redirectTo: 'wallet', pathMatch: 'full' },
  { path: 'wallet', component: WalletComponent,
    children: [
      { path: '', redirectTo:'history', pathMatch:'full' },
      { path: 'send', component: SendComponent},
      { path: 'receive', component: ReceiveComponent},
      { path: 'history', component: HistoryComponent}
    ]
  },
];

@NgModule({
  imports: [ RouterModule.forChild(routes) ],
  exports: [ RouterModule ]
})

export class WalletRoutingModule {}
