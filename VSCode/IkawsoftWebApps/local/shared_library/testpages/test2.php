<?php

    /*if (!defined('DIRECTORY_SEPARATOR')) {
        if (strtoupper(substr(PHP_OS, 0, 3)) === 'WIN') {
            define('DIRECTORY_SEPARATOR', '\\'); // Windows uses backslash
        } elseif (PHP_OS === 'Linux') {
            define('DIRECTORY_SEPARATOR', '/');  // Linux uses forward slash
        } else {
            define('DIRECTORY_SEPARATOR', '/');  // Default for Unix-like systems
        }
    }

     $gApplicationFileDirectory = DIRECTORY_SEPARATOR . substr($_SERVER['SCRIPT_FILENAME'], strlen($_SERVER['DOCUMENT_ROOT']) + 1, strpos($_SERVER['SCRIPT_FILENAME'], DIRECTORY_SEPARATOR, strlen($_SERVER['DOCUMENT_ROOT']) + 1) - (strlen($_SERVER['DOCUMENT_ROOT']) + 1)) . DIRECTORY_SEPARATOR;
     echo( $_SERVER['DOCUMENT_ROOT'] . $gApplicationFileDirectory . 'lib_include' . DIRECTORY_SEPARATOR . 'config.php');

     //require_once $_SERVER['DOCUMENT_ROOT'] . $gApplicationFileDirectory . 'lib_include' . DIRECTORY_SEPARATOR . 'config.php';
*/
?>

<!DOCTYPE html>
<html lang="en">
 <head>
   <title>Disaster Recovery Design</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
  


<?php



$server   = "ikawsoft-server.database.windows.net";
$database = "DR_Tracker";
$user     = "ShadowFalcon";
$pass     = "6qtbYm5YVFjNZwYT";
try {
    $conn = new PDO("sqlsrv:Server=$server;Database=$database", $user, $pass);
    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);

    echo "Connected successfully";
} catch (PDOException $e) {
    echo "Connection failed: " . $e->getMessage();
}

?>
 
 </head>
 
 <body style="margin-top: 0px; margin-left: 0px;">
    <?php


  echo(PHP_OS);
 

?>
<script>
    const now = new Date();
    document.write("Javascript: " + now);
</script>








     






 </body>
</html>

