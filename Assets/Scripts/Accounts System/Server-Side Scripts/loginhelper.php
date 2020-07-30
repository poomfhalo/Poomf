<?php

// LOGINHELPER.PHP
// INPUT: (username) via _POST
// FUNCTIONALITY: Tries to login with the provided username
// NOTES: Will create a new entry if the username doesn't exist
// OUTPUT: "duplicate match", "login success" or "new user"

// And some helper functions
require('helper.php');

// Load the credentials that have been set-up by Online Account System in Unity's editor
require('credentials.php');

// Connect to server and select databse.
$link = try_mysql_connect($databaseHostname, $databaseUsername, $databasePassword, $databaseDbName, $databasePort);

mysqli_set_charset($link, 'utf8');

// username sent from form
$username = $_POST['username'];

// To protect MySQL injection (more detail about MySQL injection)
$username = stripslashes($username);
$username = mysqli_real_escape_string($link, $username);

$query = "SELECT * FROM poomf WHERE username= '$username'";
$result = try_mysql_query($link, $query);

// Mysql_num_row is counting table row
$count = mysqli_num_rows($result);

if ($count == 0) {
    // No match, add a new user to the database
    $query = "INSERT INTO poomf (username) VALUES ('$username')";

    try_mysql_query($link, $query);
    echo 'new user';
    exit();
} else if ($count == 1) {

    echo 'login success';
    exit();
}
// If we found more than 1 entry
else
    die('duplicate match');
