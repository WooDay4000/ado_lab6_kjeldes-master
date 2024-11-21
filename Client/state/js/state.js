const { error } = require("jquery");

class StatePage {

    constructor() {
        this.state = {
            stateCode = "";
            stateName = "";
        };

        // instance variables that the app needs but are not part of the "state" of the application
        this.server = "http://localhost:5000/api"
        this.url = this.server + "/states";

        // instance variables related to ui elements simplifies code in other places
        this.$form = document.querySelector('#stateForm');
        this.$stateCode = document.querySelector('#stateCode');
        this.$findButton = document.querySelector('#findBtn');
        this.$addButton = document.querySelector('#addBtn');
        this.$deleteButton = document.querySelector('#deleteBtn');
        this.$editButton = document.querySelector('#editBtn');
        this.$saveButton = document.querySelector('#saveBtn');
        this.$cancelButton = document.querySelector('#cancelBtn');

        // call these methods
        // call these methods to set up the page
        this.bindAllMethods();
        this.fetchStates();
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
            fetch(`${this.url}/${this.$stateCode}`)
                .then(response => response.json())
                .then(data => {
                    if (data.status == 404) {
                        alert('That state does not exist in our database')
                    }
                    else {
                        this.state.stateName = data;
                        this.fillStateFields();
                        this.enableButtons("found");
                    }
                })
                .catch(error => {
                    alert('There was a proplem getting state info');
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
        if (this.state.stateCode != "") {
            fetch(`${this.url}/${this.state.stateCode}`, { method: 'DELETE' })
                .then(response => response.json())
                .then(data => {
                    // returns the record that we deleted so the ids should be the same 
                    if (this.state.stateCode == data.stateCode) {
                        this.state.stateCode = "";
                        this.$customerId.value = "";
                        this.clearStateFields();
                        this.enableButtons("pageLoad");
                        alert("State was deleted.")
                    }
                    else {
                        alert('There was a problem deleting state info!');
                    }
                })
                .catch(error => {
                    alert('There was a problem deleting state info!');
                });
        }
        else {
            // this should never happen if the right buttons are enabled
        }
    }

    // makes either a post or a put request to /api/customers
    // either adds a new customer or updates an existing customer in the database
    // based on the customer information in the form
    onSaveCustomer(event) {
        event.preventDefault();
        // adding
        if (this.state.stateCode == "") {
            fetch(`${this.url}`, {
                method: 'POST',
                body: JSON.stringify({
                    stateCode: this.$stateCode.value,
                    stateName: this.stateName.value,
                }),
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => response.json())
                .then(data => {
                    // returns the record that we added so the ids should be there 
                    if (data.stateCode) {
                        this.state.stateCode = data.stateCode;
                        this.stateCode.value = this.state.stateCode;
                        this.fillCustomerFields();
                        this.stateCode.readOnly = false;
                        this.enableButtons("found");
                        alert("Customer was added.")
                    }
                    else {
                        alert('There was a problem adding state info!');
                    }
                })
                .catch(error => {
                    alert('There was a problem adding state info!');
                });
        }
        // updating
        else {
            // the format of the body has to match the original object exactly
            // so make a copy of it and copy the values from the form
            // THis is where we left off.
            let state = Object.assign(this.state.customer);
            customer.name = this.$customerName.value;
            customer.address = this.$customerAddress.value;
            customer.city = this.$customerCity.value;
            customer.state = this.$customerState.value;
            customer.zipCode = this.$customerZipcode.value;
            fetch(`${this.url}/${this.state.customerId}`, {
                method: 'PUT',
                body: JSON.stringify(customer),
                headers: {
                    'Content-Type': 'application/json'
                }
            })
                .then(response => {
                    // doesn't return a body just a status code of 204 
                    if (response.status == 204) {
                        this.state.customer = Object.assign(customer);
                        this.fillCustomerFields();
                        this.$customerId.readOnly = false;
                        this.enableButtons("found");
                        alert("Customer was updated.")
                    }
                    else {
                        alert('There was a problem updating customer info!');
                    }
                })
                .catch(error => {
                    alert('There was a problem adding customer info!');
                });
        }
    }
}