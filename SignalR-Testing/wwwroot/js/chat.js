"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (msg) {
    console.log("I received a message! :" + msg);
});

connection.on("ReceivedMessageFromServer", function (msg) {
    console.log(msg);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendNotifications").addEventListener("click", function (event) {
    connection.invoke("SendMessages", "Helloworld!").catch(function (err) {
        return console.error(err.toString());
    });
});

document.getElementById("triggerAction").addEventListener("click", function (event) {
    connection.invoke("TriggerAction", "exec").catch(function (err) {
        return console.error(err.toString());
    });
});

var notificationHub = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

notificationHub.start();
notificationHub.on("ReceivedMessageFromServer", function (data) {
    console.log(data);
});

document.getElementById("registerToGroup1").addEventListener("click", function (event) {
    notificationHub.invoke("JoinGroup", "Grp1").catch(function (err) {
        return console.error(err.toString());
    });
});