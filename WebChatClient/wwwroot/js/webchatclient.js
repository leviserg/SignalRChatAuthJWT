
function scrollToBottom(elem) {
    elem.scrollTop = elem.scrollHeight;
}

let subscribed = false;
let serverUrl = "https://localhost:7065";

document.querySelector('#name').value = "";

let userName = prompt("Please enter your name:", "alex_777");

$(document).ready(function () {
    $("#name").val(userName);
});

let connection = null;
let _token = "";

function initConnection() {

    connection = new signalR.HubConnectionBuilder()
        .withUrl(serverUrl + "/chat/?username=" + $("#name").val() + "&token=" + _token)
        //,{
        //    headers: { "username" : userName },
        //    transport: signalR.HttpTransportType.LongPolling 
        //})
        .withAutomaticReconnect()
        .build();

    connection.on('SendClientMessageToChat', (message) => {
        appendMessage(response_time_format(message.createdAt) + " : " + message.caller, message.text, 'black');
    });

    connection.onclose(error => {
        console.log('Connection closed. ', error)
    });

    connection.onreconnecting(error => {
        console.log('Connection reconnecting. ', error);
    });

    connection.onreconnected(connectionId => {
        console.log('Connectin reconnected with id: ', connectionId);
    });
}

function appendMessage(sender, message, color) {
    //document.querySelector('#messages-content').insertAdjacentHTML("beforeend", `<div style="color:${color}" class="py-0,my-0">${sender} : ${message}<br></div><br>`);
    document.querySelector('#messages-content').value += sender + ' : ' + message + '\n';
    scrollToBottom(document.querySelector('#messages-content'));
}

async function connect() {

    if (connection === null) {
        initConnection();
    }

    if (connection.state === 'Disconnected') {
        try {
            await connection.start();
        }
        catch (error) {
            console.log(error);
        }
        if (connection.state === 'Connected') {
            document.querySelector('#conState').textContent = 'Connected';
            document.querySelector('#conState').style.color = 'green';
            document.querySelector('#connectButton').textContent = 'Disconnect';
        }
    } else if (connection.state === 'Connected') {
        await connection.stop();
        document.querySelector('#conState').textContent = 'Disconnected';
        document.querySelector('#conState').style.color = 'red';
        document.querySelector('#connectButton').textContent = 'Connect';
    }
};

async function sendMessage() {
    if (connection.state === 'Connected') {
        let textArea = document.querySelector('#message');
        let message = textArea.value;
        try {
            await connection.send('AddMessageToChat', message);
            let d = new Date();
            appendMessage(time_format(d) + ' : Me', message, 'green');
        }
        catch (error) {
            console.log(error);
        }
        document.querySelector('#message').value = '';
    }
}

async function subscribe() {
    if (connection.state === 'Connected') {
        if (subscribed == true) {
            try {
                await connection.invoke("Unsubscribe");
                subscribed = false;
                document.querySelector('#subscribeButton').textContent = 'Subscribe';
            }
            catch (error) {
                console.log(error);
            }
        }
        else {
            try {
                await connection.invoke("Subscribe");
                subscribed = true;
                document.querySelector('#subscribeButton').textContent = 'Unsubscribe';
            }
            catch (error) {
                console.log(error);
            }
        }
    }
}

function applyToken() {
    getToken(function (data) {
        if (data != null && data.length > 0) {
            alert("Success login...");
            _token = data;
            initConnection();
        }
        else {
            alert("Wrong credentials!");
        }
    });
}

function getToken(callbackFn) {
    let formData = { "Login": $("#name").val(), "Password" : $("#userpassword").val() };
    try {
        $.ajax({
            type: 'POST',
            url: serverUrl + "/api/auth/token",
            data: JSON.stringify(formData),
            contentType: "application/json; charset=utf-8",
            //dataType: "json",
            //processData: false,
            success: function (response) {
                callbackFn(response)
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
    catch (e) {
        console.log(e);
    }
}

function time_format(d) {
    hours = format_two_digits(d.getHours());
    minutes = format_two_digits(d.getMinutes());
    seconds = format_two_digits(d.getSeconds());
    return hours + ":" + minutes + ":" + seconds;
}

function format_two_digits(n) {
    return n < 10 ? '0' + n : n;
}

function response_time_format(d) {
    return d.substr(d.indexOf("T") + 1, 8); //.inhours + ":" + minutes + ":" + seconds;
}