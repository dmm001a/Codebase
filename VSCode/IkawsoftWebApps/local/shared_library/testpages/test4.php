<!DOCTYPE html>
<html lang="en">
 <head>
   <title>Disaster Recovery Design</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    
    
    <link href="https://cdn.jsdelivr.net/npm/tom-select@2.4.3/dist/css/tom-select.css" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/tom-select@2.4.3/dist/js/tom-select.complete.min.js"></script>
     
 
 </head>
 
 <body style="margin-top: 0px; margin-left: 0px;">
    
<?php

    if (!defined('DIRECTORY_SEPARATOR')) {
        if (strtoupper(substr(PHP_OS, 0, 3)) === 'WIN') {
            define('DIRECTORY_SEPARATOR', '\\'); // Windows uses backslash
        } elseif (PHP_OS === 'Linux') {
            define('DIRECTORY_SEPARATOR', '/');  // Linux uses forward slash
        } else {
            define('DIRECTORY_SEPARATOR', '/');  // Default for Unix-like systems
        }
    }

    $sApplicationFileDirectory = "";
    $sIncludeFilePath = "";

    $sApplicationFileDirectory = getenv('Application_File_Directory');
    $sIncludeFilePath = $_SERVER['DOCUMENT_ROOT'] . $sApplicationFileDirectory . 'app_include' . DIRECTORY_SEPARATOR . 'lib_include' . DIRECTORY_SEPARATOR . 'config.php';
    
     require_once $sIncludeFilePath;

// Import the Postmark Client Class:




// Usage example
$mailService = new MailBabyService();
$result = $mailService->sendEmail(
    'devon.manelski83@outlook.com',
    'DR Tracker Login Information',
    '<p>Hello,</p><p>This is a system notification from DR Tracker confirming that email delivery is functioning normally.</p><p>Regards,<br>Devon</p>'
);

print_r($result);
die();

?>


 </body>
</html>

