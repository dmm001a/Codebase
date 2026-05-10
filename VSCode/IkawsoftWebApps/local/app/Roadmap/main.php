<?php


    require_once $_SERVER['DOCUMENT_ROOT'] . DIRECTORY_SEPARATOR . 'startup.php';

    mIncludePage(enuIncludePackageID::InitConfig, mGetApplicationEnvironment($_SERVER['REQUEST_URI']));    

?>

<!DOCTYPE html>
<html lang="en">
 <head>
   <title>Roadmap Builder</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <?php echo(mOutputHTMLHead()); ?>
 
 </head>
 
 <body style="margin-top: 0px; margin-left: 0px;">
    
        <?php

            $sIncludeFilePath = mGetRootPath(ApplicationID::Roadmap, enuPathRootType::File, enuPathType::ApplicationFolder, enuFolderType::ui) . 'header.php';
            require_once $sIncludeFilePath;

        ?>

        <div class="table-container">
                

                
                
               
                
         </div>     

	<!-- Standard Table -->           
	<table id="StandardTable"  class="table table-bordered table-hover">
	    <thead class="table-header">
	    </thead>
	    <tbody class="table-body">
	    </tbody>
	</table>
        </div>
        <?php
            /*Output the status bar with the version number*/
            $sVersionNumber = "";
            $sApplicationEnvironment = "";
            
            $sVersionNumber = mGetApplicationVersionNumber();
            $sApplicationEnvironment = mGetApplicationEnvironment($_SERVER['REQUEST_URI']);
            
            echo("
                    <div id='CustomStatusBar'>
                        <div id='status-left' class='status-left'>Environment: $sApplicationEnvironment</div>
                        <div id='status-center' class='status-center'></div>
                        <div id='status-right' class='status-right'>Version $sVersionNumber</div>
                    </div>
                ");


        ?>

        
        
    <?php
        //Include JS Include.php
        $sQueryStringVariable = "&AppID=" . urlencode(ApplicationID::Roadmap->value);
        $sURL = mGetRootPath(ApplicationID::Roadmap, enuPathRootType::URL, enuPathType::SharedLibraryFolder, enuFolderType::lib) . "js_include.php?v=1.0";

        $sURL = $sURL . $sQueryStringVariable;

        echo("<script src='" . $sURL . "'></script>");
    ?>
   
    


    

 </body>
</html>

