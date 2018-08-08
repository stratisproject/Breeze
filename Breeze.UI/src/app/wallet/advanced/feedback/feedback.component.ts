import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-feedback',
  templateUrl: './feedback.component.html',
  styleUrls: ['./feedback.component.css']
})
export class FeedbackComponent {
    private _loading = false;
    private _errored = false;
    private _erroredText = "Failed";
    private _success = false;
    private _showSuccessTick = false;

    @Input()
    public set loading(value: boolean) {
        this._loading = value;
    }
    public get loading(): boolean {
        return this._loading;
    } 

    @Input()
    public set errored(value: boolean) {
        this._errored = value;
    }
    public get errored(): boolean {
        return this._errored;
    }

    @Input()
    public set erroredText(value: string) {
        this._erroredText = value;
    }
    public get erroredText(): string {
        return this._erroredText;
    }

    @Input()
    public set success(value: boolean) {
        this._success = value;
    }
    public get success(): boolean {
        return this._success;
    }

    @Input()
    public set showSuccessTick(value: boolean) {
        this._showSuccessTick = value;
    }
    public get showSuccessTick(): boolean {
        return this._showSuccessTick;
    }
}
