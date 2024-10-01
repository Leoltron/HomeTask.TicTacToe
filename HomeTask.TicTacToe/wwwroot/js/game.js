"use strict";

document.getElementById("newGame").addEventListener("click", function (event) {
    event.preventDefault();
    document.cookie = "GameId=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    location.reload();
})

const connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();
const gameContext = JSON.parse(document.getElementById('gameContainer').getAttribute('data-game'));

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("PlayerMoved", function (role, x, y) {
    const playerMoved = role === 0 ? "X" : "O";
    console.log(`Player ${playerMoved} moved at ${x}, ${y}`);
    document.querySelector(`.cell[data-x="${x}"][data-y="${y}"]`).setAttribute('data-state', playerMoved);
});

connection.on("GameStateUpdated", function (state) {
    console.log("New state: " + state);
    if (state === "XWin" || state === "OWin" || state === "Draw") {
        document.getElementById('stateMessage').innerText = state;
    }
});

connection.on("Player1RoleSelected", function (role) {
    console.log("Role for Player 1: " + role);
    document.querySelector('.role-selector').classList.add("hide");

    if (gameContext["IsPlayer1"] === false) {
        role = role === "X" ? "O" : "X";
    }

    document.getElementById("selectedMessage").innerText = "You are " + role;
})

document.getElementById("selectX")?.addEventListener("click", function (event) {
    selectRole("X");
    event.preventDefault();
});

document.getElementById("selectO")?.addEventListener("click", function (event) {
    selectRole("O");
    event.preventDefault();
});

document.querySelectorAll("[data-cell]").forEach(function (cell) {
    const x = Number.parseInt(cell.getAttribute("data-x"));
    const y = Number.parseInt(cell.getAttribute("data-y"));
    cell.addEventListener("click", function (event) {
        connection.invoke("MakeMove", x, y).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    })
})

function selectRole(roleString) {
    connection.invoke("SelectRole", roleString).catch(function (err) {
        return console.error(err.toString());
    });
}