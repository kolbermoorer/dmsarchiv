function User() {
    this.id = null;
    this.name = null;
    this.username = null;

    this.login("Thomas", "");
}

User.prototype.login = function (username, password) {
    this.id = 2
    this.name = "Thomas";
    this.username = "kolbermoorer";
};

