import { Component, OnInit, Input } from '@angular/core';

import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-generic-modal',
  templateUrl: './generic-modal.component.html',
  styleUrls: ['./generic-modal.component.css']
})
export class GenericModalComponent implements OnInit {

  @Input() public title: string = "Something went wrong";
  @Input() public body: string = "Something went wrong while connecting to the API. Please restart the application.";

  constructor(public activeModal: NgbActiveModal) {}

  ngOnInit() {
  }
}
