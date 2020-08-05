<?php

// LOGINHELPER.PHP
// INPUT: (username) via _POST
// FUNCTIONALITY: Checks if the supplied username exists or not
// OUTPUT:  0 for new user, 1 for existing user

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

$query = "SELECT * FROM es3cloud WHERE user= '$username'";
$result = try_mysql_query($link, $query);

// Mysql_num_row is counting table row
$count = mysqli_num_rows($result);

if ($count == 0) {
    // No match, this is a new user
    die("0");
} else {
    // Existing user
    die("1");
}
