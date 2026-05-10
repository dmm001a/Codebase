<html lang="en">
 <head>
   <title>Disaster Recovery Error Page</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
 </head>
 
 <body style="margin-top: 0px; margin-left: 0px;">
<table style="background-color:#2e86c1;width: 100%; height: 40px;">
    <tr>
        <td style="width: 100%; height: 20px;">
            <p style="color: #ffffff;font-size:24px;margin-left:20px;">Disaster Recovery Tracker</p>
        </td>
    </tr>
</table>

    <br/>
    <p>
        An error has occurred in the Disaster Recovery Tracker.  The error has been logged in the application error log.  The error details are as follows.  
        
        <br><br>Please contact gbs-es-disaster-recovery@wolterskluwer.com.</br>
        <br/>
        <?php 

            $sErrorMessage = "";

            $sErrorMessage = urldecode($_GET['ErrorMessage'] ?? '');

            echo("Error Message: " . $sErrorMessage);


        ?>
    </p>
        

 
 </body>
</html>