import { Directive } from '@angular/core';
import { AbstractControl } from '@angular/forms';

@Directive({
  selector: '[appPasswordValidation]'
})
export class PasswordValidationDirective {
  constructor() { }

  static MatchPassword(AC: AbstractControl) {
    AC.get('walletPassword').valueChanges.subscribe(() => {
      AC.get('walletPasswordConfirmation').updateValueAndValidity();
    });

    let password = AC.get('walletPassword').value;
    let confirmPassword = AC.get('walletPasswordConfirmation').value;

    if(password != confirmPassword) {
      AC.get('walletPasswordConfirmation').setErrors({ walletPasswordConfirmation: true });
    } else {
      return null
    }
  }
}
