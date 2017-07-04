import { Directive, Renderer, ElementRef, OnInit } from '@angular/core';

@Directive({
  selector: '[myAutoFocus]'
})
export class AutoFocusDirective implements OnInit {

  constructor(private renderer: Renderer, private elementRef: ElementRef) { }

  ngOnInit() {
    this.renderer.invokeElementMethod(
      this.elementRef.nativeElement, 'focus', []
    );
  }
}
