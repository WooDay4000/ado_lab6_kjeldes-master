class StatePage {
    constructor() {
        this.state = {
            stateCode: "",
            stateName: ""
        };

        // instance variables that the app needs but are not part of the "state" of the application
        this.server = "http://localhost:5000/api"
        this.url = this.server + "/states";

        // instance variables related to ui elements simplifies code in other places
        this.$form = document.querySelector('#stateForm');
        this.$stateCode = document.querySelector('#stateCode');
        this.$stateName = document.querySelector('#stateName');
        this.$findButton = document.querySelector('#findBtn');
        this.$addButton = document.querySelector('#addBtn');
        this.$deleteButton = document.querySelector('#deleteBtn');
        this.$editButton = document.querySelector('#editBtn');
        this.$saveButton = document.querySelector('#saveBtn');
        this.$cancelButton = document.querySelector('#cancelBtn');

        // call these methods to set up the page
        this.bindAllMethods();
        this.makeFieldsReadOnly(true);
        this.makeFieldsRequired(false);
        this.enableButtons("pageLoad");
    }
    // any method that is used as part of an event handler must bind this to the class
    // not all of these methods need to be bound but it was easier to do them all as I wrote them
    bindAllMethods() {
        this.onFindState = this.onFindState.bind(this);
        this.onEditState = this.onEditState.bind(this);
        this.onCancel = this.onCancel.bind(this);
        this.onDeleteState = this.onDeleteState.bind(this);
        this.onSaveState = this.onSaveState.bind(this);
        this.onAddState = this.onAddState.bind(this);

        this.makeFieldsReadOnly = this.makeFieldsReadOnly.bind(this);
        this.makeFieldsRequired = this.makeFieldsRequired.bind(this);
        this.fillStateFields = this.fillStateFields.bind(this);
        this.clearStateFields = this.clearStateFields.bind(this);
        this.disableButtons = this.disableButtons.bind(this);
        this.disableButton = this.disableButton.bind(this);
        this.enableButtons = this.enableButtons.bind(this);
    }

    // makes an api call to /api/state/# where # is the primary key of the state
    // finds a state based on state code.
    onFindState(event) {
        event.preventDefault();
        if (this.$stateCode.value != "") {
            this.state.stateCode = this.$stateCode.value;
            fetch(`${this.url}/${this.state.stateCode}`)
                .then(response => response.json())
                .then(data => {
                    if (data.status == 404) {
                        alert('That state does not exist in our database');
                    }
                    else {
                        this.state.stateName = data.stateName;
                        this.fillStateFields();
                        this.enableButtons("found");
                    }
                })
                .catch(error => {
                    alert('There was a problem getting state info!');
                });
        }
        else {
            this.clearStateFields();
        }
    }

    // makes a delete request to /api/state/# where # is the primary key of the state
    // deletes the state displayed on the screen from the database
    onDeleteState(event) {
        event.preventDefault();
        if (this.state.stateCode == "") {
            alert('There was a problem deleting state info, enter a state code!');
            return;
        }
        fetch(`${this.url}/${this.state.stateCode}`, { method: 'DELETE' })
            .then(response => {
                if (response.ok) {
                    this.state.stateCode = "";
                    this.$stateCode.value = "";
                    this.clearStateFields();
                    this.enableButtons("pageLoad");
                    alert("State was deleted");
                }
                else {
                    alert('There was a problem deleting state info!');
                }
            })
            .catch(error => {
                alert('There was a problem deleting state info!');
            });
    }

    // makes either a post or a put request to /api/states
    // either adds a new state or updates an existing state in the database
    // based on the state information in the form
    onSaveState(event) {
        event.preventDefault();
        // ---
        const stateCode = this.$stateCode.value;
        const stateName = this.$stateName.value;
        const requestBody = { stateCode, stateName };
        // ---
        fetch(`${this.url}/${stateCode}`)
            .then(response => {
                // Add
                if (response.status == 404) {
                    fetch(`${this.url}`, {
                        method: 'POST',
                        body: JSON.stringify(requestBody),
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    })
                        .then(response => response.json())
                        .then(data => {
                            if (data.stateCode) {
                                this.state.stateCode = data.stateCode;
                                this.state.stateName = data.stateName;
                                this.fillStateFields();
                                this.$stateCode.readOnly = false;
                                this.enableButtons("found");
                                alert("State was added.")
                            }
                            else {
                                alert('There was a problem adding state info!');
                            }
                        })
                        .catch(error => {
                            alert('There was a problem adding state info!');
                        })
                } else {
                    // Update
                    // the format of the body has to match the original object exactly 
                    // so make a copy of it and copy the values from the form
                    fetch(`${this.url}/${stateCode}`, {
                        method: 'PUT',
                        body: JSON.stringify(requestBody),
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    })
                        .then(response => {
                            if (response.ok) {
                                this.fillStateFields();
                                this.$stateCode.readOnly = false;
                                this.enableButtons("found");
                                alert("State was updated.")
                            }
                            else {
                                alert('There was a problem updating state info!');
                            }
                        })
                }
            })
            .catch(error => {
                alert('There was a problem updating state info!');
            })
    }


    // makes the fields editable
    onEditState(event) {
        event.preventDefault();
        // can't edit the StateCode
        this.$stateCode.readOnly = true;
        this.makeFieldsReadOnly(false);
        this.makeFieldsRequired(true);
        this.enableButtons("editing");
    }

    // clears the form for input of a new state
    onAddState(event) {
        event.preventDefault();
        this.clearStateFields();
        this.makeFieldsReadOnly(false);
        this.makeFieldsRequired(true);
        this.enableButtons("editing");
    }

    // cancels the editing for either a new state or an existing state
    onCancel(event) {
        event.preventDefault();
        if (this.state.stateCode == "") {
            this.clearStateFields();
            this.makeFieldsReadOnly();
            this.makeFieldsRequired(false);
            this.$stateCode.readOnly = false;
            this.enableButtons("pageLoad");
        }
        else {
            this.fillStateFields();
            this.$stateCode.readOnly = false;
            this.enableButtons("found");
        }
    }

    // fills the form with data based on the state
    fillStateFields() {
        // fill the fields
        this.$stateCode.value = this.state.stateCode;
        this.$stateName.value = this.state.stateName;
        this.makeFieldsReadOnly();
    }

    // clears the ui
    clearStateFields() {
        this.$stateCode.value = "";
        this.$stateName.value = "";
    }

    //enables or disables ui elements
    makeFieldsReadOnly(readOnly = true) {
        this.$stateName.readOnly = readOnly;
    }

    // makes ui elements required when editing
    makeFieldsRequired(required = true) {
        this.$stateCode.required = required;
        this.$stateName.required = required;
    }

    // disables an array of buttons
    disableButtons(buttons) {
        buttons.forEach(b => b.onclick = this.disableButton);
        buttons.forEach(b => b.classList.add("disabled"));
    }

    // disables one button
    disableButton(event) {
        event.preventDefault();
    }

    // enables ui elements based on the editing state of the page
    enableButtons(state) {
        switch (state) {
            case "pageLoad":
                this.disableButtons([this.$deleteButton, this.$editButton, this.$saveButton, this.$cancelButton]);
                this.$findButton.onclick = this.onFindState;
                this.$findButton.classList.remove("disabled");
                this.$addButton.onclick = this.onAddState;
                this.$addButton.classList.remove("disabled");
                break;
            case "editing": case "adding":
                this.disableButtons([this.$deleteButton, this.$editButton, this.$addButton]);
                this.$saveButton.onclick = this.onSaveState;
                this.$cancelButton.onclick = this.onCancel;
                [this.$saveButton, this.$cancelButton].forEach(b => b.classList.remove("disabled"));
                break;
            case "found":
                this.disableButtons([this.$saveButton, this.$cancelButton]);
                this.$findButton.onclick = this.onFindState;
                this.$editButton.onclick = this.onEditState;
                this.$deleteButton.onclick = this.onDeleteState;
                this.$addButton.onclick = this.onAddState;
                [this.$findButton, this.$editButton, this.$deleteButton, this.$addButton].forEach(b => b.classList.remove("disabled"));
                break;
            default:
        }
    }
}

// instantiate the js app when the html page has finished loading
window.addEventListener("load", () => new StatePage());