import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SetupComponent } from './setup.component';
import { CreateComponent } from './create/create.component';
import { ShowMnemonicComponent } from './create/show-mnemonic/show-mnemonic.component';
import { ConfirmMnemonicComponent } from './create/confirm-mnemonic/confirm-mnemonic.component';
import { RecoverComponent } from './recover/recover.component';

const routes: Routes = [
  { path: '', redirectTo: 'setup', pathMatch: 'full'},
  { path: 'setup', component: SetupComponent },
  { path: 'create', component: CreateComponent },
  { path: 'create/show-mnemonic', component: ShowMnemonicComponent },
  { path: 'create/confirm-mnemonic', component: ConfirmMnemonicComponent },
  { path: 'recover', component: RecoverComponent }
];

@NgModule({
  imports: [ RouterModule.forChild(routes) ],
  exports: [ RouterModule ]
})

export class SetupRoutingModule {}
