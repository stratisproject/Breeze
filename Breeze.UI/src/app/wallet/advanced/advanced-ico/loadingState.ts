export class LoadingState {
    private _loading = false;
    private _errored = false;
    private _erroredText = "";

    public get erroredText(): string {
        return this._erroredText;
    }
    public set erroredText(value: string) {
        this._erroredText = value;
    }

    public get loading(): boolean {
        return this._loading;
    }
    public set loading(value: boolean) {
        this._loading = value;
        if (this._loading) {
            this._errored = false;
        }
    }

    public get errored(): boolean {
        return this._errored;
    }
    public set errored(value: boolean) {
        this._errored = value;
        if (this._errored) {
            this._loading = false;
        }
    }

    public get success(): boolean {
        return !this._loading && !this._errored;
    }
}
